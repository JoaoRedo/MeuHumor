using MeuHumor.Api.DTOs;
using MeuHumor.Api.Entities;
using MeuHumor.Api.Exceptions;
using MeuHumor.Api.Repositories;

namespace MeuHumor.Api.Services;

public class MoodService : IMoodService
{
    private readonly IMoodEntryRepository _moodEntryRepository;
    private readonly IMoodTypeRepository _moodTypeRepository;
    private readonly IUserRepository _userRepository;

    public MoodService(
        IMoodEntryRepository moodEntryRepository,
        IMoodTypeRepository moodTypeRepository,
        IUserRepository userRepository)
    {
        _moodEntryRepository = moodEntryRepository;
        _moodTypeRepository = moodTypeRepository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<MoodTypeDto>> GetMoodTypesAsync(CancellationToken cancellationToken = default)
    {
        var types = await _moodTypeRepository.GetActiveAsync(cancellationToken);
        return types.Select(MapToDto).ToList();
    }

    public async Task<MoodEntryResponseDto> RegisterMoodAsync(
        Guid userId, CreateMoodEntryDto dto, CancellationToken cancellationToken = default)
    {
        var moodType = await _moodTypeRepository.GetActiveByIdAsync(dto.Humor, cancellationToken)
            ?? throw new InvalidMoodTypeException(dto.Humor);

        var data = dto.Data ?? GetTodayDate();

        if (await _moodEntryRepository.ExistsForDateAsync(userId, data, cancellationToken))
            throw new DuplicateMoodEntryException(data);

        await _userRepository.EnsureExistsAsync(userId, email: null, cancellationToken);

        var entry = new MoodEntry
        {
            UserId = userId,
            Data = data,
            Humor = moodType.Id,
            Observacao = dto.Observacao
        };

        var created = await _moodEntryRepository.CreateAsync(entry, cancellationToken);
        return MapToResponse(created, moodType);
    }

    public async Task<MoodEntryResponseDto> UpdateTodayMoodAsync(
        Guid userId, UpdateMoodEntryDto dto, CancellationToken cancellationToken = default)
    {
        var moodType = await _moodTypeRepository.GetActiveByIdAsync(dto.Humor, cancellationToken)
            ?? throw new InvalidMoodTypeException(dto.Humor);

        var today = GetTodayDate();
        var existing = await _moodEntryRepository.GetByUserAndDateAsync(userId, today, cancellationToken)
            ?? throw new MoodEntryNotFoundException(today);

        var updated = await _moodEntryRepository.UpdateAsync(
            existing.Id, userId, today, moodType.Id, dto.Observacao, cancellationToken)
            ?? throw new MoodEntryNotEditableException(existing.Data);

        return MapToResponse(updated, moodType);
    }

    public async Task<IReadOnlyList<MoodEntryResponseDto>> GetHistoryAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        var entries = await _moodEntryRepository.GetHistoryByUserAsync(userId, cancellationToken);
        return entries.Select(MapToResponse).ToList();
    }

    public async Task<MonthSummaryDto> GetMonthSummaryAsync(
        Guid userId, short mes, short ano, CancellationToken cancellationToken = default)
    {
        ValidateMonthYear(mes, ano);

        var entries = await _moodEntryRepository.GetByUserAndMonthAsync(userId, mes, ano, cancellationToken);
        var activeTypes = await _moodTypeRepository.GetActiveAsync(cancellationToken);
        var contagem = activeTypes.ToDictionary(t => t.Label, _ => 0);

        foreach (var entry in entries)
        {
            var label = entry.HumorLabel;
            if (!contagem.ContainsKey(label))
                contagem[label] = 0;

            contagem[label]++;
        }

        var total = entries.Count;
        var media = total > 0
            ? Math.Round(entries.Average(e => (decimal)e.Humor), 2)
            : 0m;

        return new MonthSummaryDto
        {
            Mes = mes,
            Ano = ano,
            MediaHumor = media,
            TotalDiasRegistrados = total,
            ContagemPorCategoria = contagem
        };
    }

    private static DateOnly GetTodayDate() => DateOnly.FromDateTime(DateTime.UtcNow);

    private static void ValidateMonthYear(short mes, short ano)
    {
        if (mes is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(mes), "Mês deve estar entre 1 e 12.");

        if (ano < 2000)
            throw new ArgumentOutOfRangeException(nameof(ano), "Ano inválido.");
    }

    private static MoodTypeDto MapToDto(MoodType type) => new()
    {
        Id = type.Id,
        Label = type.Label,
        Emoji = type.Emoji,
        Ordem = type.Ordem
    };

    private static MoodEntryResponseDto MapToResponse(MoodEntryWithType entry) => new()
    {
        Id = entry.Id,
        Data = entry.Data,
        Humor = entry.Humor,
        HumorLabel = entry.HumorLabel,
        HumorEmoji = entry.HumorEmoji,
        Observacao = entry.Observacao,
        CriadoEm = entry.CriadoEm
    };

    private static MoodEntryResponseDto MapToResponse(MoodEntry entry, MoodType moodType) => new()
    {
        Id = entry.Id,
        Data = entry.Data,
        Humor = entry.Humor,
        HumorLabel = moodType.Label,
        HumorEmoji = moodType.Emoji,
        Observacao = entry.Observacao,
        CriadoEm = entry.CriadoEm
    };
}
