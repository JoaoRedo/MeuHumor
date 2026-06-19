using MeuHumor.Api.DTOs;

namespace MeuHumor.Api.Services;

public interface IMoodService
{
    Task<IReadOnlyList<MoodTypeDto>> GetMoodTypesAsync(CancellationToken cancellationToken = default);
    Task<MoodEntryResponseDto?> GetTodayMoodAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<MoodEntryResponseDto> RegisterMoodAsync(Guid userId, CreateMoodEntryDto dto, CancellationToken cancellationToken = default);
    Task<MoodEntryResponseDto> UpdateTodayMoodAsync(Guid userId, UpdateMoodEntryDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MoodEntryResponseDto>> GetHistoryAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<MonthSummaryDto> GetMonthSummaryAsync(Guid userId, short mes, short ano, CancellationToken cancellationToken = default);
}
