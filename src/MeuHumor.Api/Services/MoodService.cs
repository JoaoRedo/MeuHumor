using MeuHumor.Api.DTOs;
using MeuHumor.Api.Entities;
using MeuHumor.Api.Exceptions;
using MeuHumor.Api.Repositories;

namespace MeuHumor.Api.Services;

public class MoodService : IMoodService
{
    private readonly IMoodEntryRepository _moodEntryRepository;
    private readonly IUserRepository _userRepository;

    public MoodService(IMoodEntryRepository moodEntryRepository, IUserRepository userRepository)
    {
        _moodEntryRepository = moodEntryRepository;
        _userRepository = userRepository;
    }

    public async Task<MoodEntryResponseDto> RegisterMoodAsync(
        Guid userId, CreateMoodEntryDto dto, CancellationToken cancellationToken = default)
    {
        var data = dto.Data ?? DateOnly.FromDateTime(DateTime.UtcNow);

        if (await _moodEntryRepository.ExistsForDateAsync(userId, data, cancellationToken))
            throw new DuplicateMoodEntryException(data);

        await _userRepository.EnsureExistsAsync(userId, email: null, cancellationToken);

        var entry = new MoodEntry
        {
            UserId = userId,
            Data = data,
            Humor = dto.Humor,
            Observacao = dto.Observacao
        };

        var created = await _moodEntryRepository.CreateAsync(entry, cancellationToken);
        return MapToResponse(created);
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

        var contagem = Enum.GetValues<MoodLevel>()
            .ToDictionary(level => level.ToLabel(), _ => 0);

        foreach (var entry in entries)
        {
            var label = ((MoodLevel)entry.Humor).ToLabel();
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

    private static void ValidateMonthYear(short mes, short ano)
    {
        if (mes is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(mes), "Mês deve estar entre 1 e 12.");

        if (ano < 2000)
            throw new ArgumentOutOfRangeException(nameof(ano), "Ano inválido.");
    }

    private static MoodEntryResponseDto MapToResponse(MoodEntry entry) => new()
    {
        Id = entry.Id,
        Data = entry.Data,
        Humor = entry.Humor,
        HumorLabel = ((MoodLevel)entry.Humor).ToLabel(),
        Observacao = entry.Observacao,
        CriadoEm = entry.CriadoEm
    };
}
