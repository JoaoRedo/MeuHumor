using MeuHumor.Api.Entities;

namespace MeuHumor.Api.Repositories;

public interface IMoodEntryRepository
{
    Task<bool> ExistsForDateAsync(Guid userId, DateOnly data, CancellationToken cancellationToken = default);
    Task<MoodEntry?> GetByUserAndDateAsync(Guid userId, DateOnly data, CancellationToken cancellationToken = default);
    Task<MoodEntry> CreateAsync(MoodEntry entry, CancellationToken cancellationToken = default);
    Task<MoodEntry?> UpdateAsync(Guid id, Guid userId, DateOnly data, short humor, string? observacao, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MoodEntryWithType>> GetHistoryByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MoodEntryWithType>> GetByUserAndMonthAsync(Guid userId, short mes, short ano, CancellationToken cancellationToken = default);
}
