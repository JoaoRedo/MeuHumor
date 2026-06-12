using MeuHumor.Api.Entities;

namespace MeuHumor.Api.Repositories;

public interface IMoodTypeRepository
{
    Task<IReadOnlyList<MoodType>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MoodType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MoodType?> GetActiveByIdAsync(short id, CancellationToken cancellationToken = default);
}
