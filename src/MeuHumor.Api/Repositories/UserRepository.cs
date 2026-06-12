using Dapper;
using MeuHumor.Api.Configuration;
using MeuHumor.Api.Entities;
using Microsoft.Extensions.Options;
using Npgsql;

namespace MeuHumor.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IOptions<DatabaseSettings> databaseSettings)
    {
        _connectionString = databaseSettings.Value.ConnectionString;
    }

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        const string sql = """
            SELECT id          AS Id,
                   nome        AS Nome,
                   email       AS Email,
                   telefone    AS Telefone,
                   criado_em   AS CriadoEm,
                   atualizado_em AS AtualizadoEm
            FROM public.users
            WHERE id = @UserId
            """;

        return await connection.QuerySingleOrDefaultAsync<User>(
            new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));
    }

    public async Task EnsureExistsAsync(Guid userId, string? email, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        const string sql = """
            INSERT INTO public.users (id, email)
            VALUES (@UserId, @Email)
            ON CONFLICT (id) DO NOTHING
            """;

        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { UserId = userId, Email = email }, cancellationToken: cancellationToken));
    }
}
