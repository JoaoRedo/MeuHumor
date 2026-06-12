using MeuHumor.Api.Entities;

namespace MeuHumor.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task EnsureExistsAsync(Guid userId, string? email, CancellationToken cancellationToken = default);
}
