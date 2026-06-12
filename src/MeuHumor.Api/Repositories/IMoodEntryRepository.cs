using MeuHumor.Api.Entities;

namespace MeuHumor.Api.Repositories;

public interface IMoodEntryRepository
{
    Task<bool> ExistsForDateAsync(Guid userId, DateOnly data, CancellationToken cancellationToken = default);
    Task<MoodEntry> CreateAsync(MoodEntry entry, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MoodEntry>> GetHistoryByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MoodEntry>> GetByUserAndMonthAsync(Guid userId, short mes, short ano, CancellationToken cancellationToken = default);
}
