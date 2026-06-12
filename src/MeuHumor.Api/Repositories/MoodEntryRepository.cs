using Dapper;
using MeuHumor.Api.Configuration;
using MeuHumor.Api.Entities;
using Microsoft.Extensions.Options;
using Npgsql;

namespace MeuHumor.Api.Repositories;

public class MoodEntryRepository : IMoodEntryRepository
{
    private readonly string _connectionString;

    public MoodEntryRepository(IOptions<DatabaseSettings> databaseSettings)
    {
        _connectionString = databaseSettings.Value.ConnectionString;
    }

    public async Task<bool> ExistsForDateAsync(Guid userId, DateOnly data, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        const string sql = """
            SELECT EXISTS(
                SELECT 1
                FROM public.mood_entries
                WHERE user_id = @UserId AND data = @Data
            )
            """;

        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { UserId = userId, Data = data }, cancellationToken: cancellationToken));
    }

    public async Task<MoodEntry> CreateAsync(MoodEntry entry, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        const string sql = """
            INSERT INTO public.mood_entries (id, user_id, data, humor, observacao)
            VALUES (@Id, @UserId, @Data, @Humor, @Observacao)
            RETURNING id            AS Id,
                      user_id       AS UserId,
                      data          AS Data,
                      humor         AS Humor,
                      observacao    AS Observacao,
                      criado_em     AS CriadoEm,
                      atualizado_em AS AtualizadoEm
            """;

        entry.Id = entry.Id == Guid.Empty ? Guid.NewGuid() : entry.Id;

        var created = await connection.QuerySingleAsync<MoodEntry>(
            new CommandDefinition(sql, entry, cancellationToken: cancellationToken));

        return created;
    }

    public async Task<IReadOnlyList<MoodEntry>> GetHistoryByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        const string sql = """
            SELECT id            AS Id,
                   user_id       AS UserId,
                   data          AS Data,
                   humor         AS Humor,
                   observacao    AS Observacao,
                   criado_em     AS CriadoEm,
                   atualizado_em AS AtualizadoEm
            FROM public.mood_entries
            WHERE user_id = @UserId
            ORDER BY data DESC
            """;

        var entries = await connection.QueryAsync<MoodEntry>(
            new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));

        return entries.AsList();
    }

    public async Task<IReadOnlyList<MoodEntry>> GetByUserAndMonthAsync(
        Guid userId, short mes, short ano, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        const string sql = """
            SELECT id            AS Id,
                   user_id       AS UserId,
                   data          AS Data,
                   humor         AS Humor,
                   observacao    AS Observacao,
                   criado_em     AS CriadoEm,
                   atualizado_em AS AtualizadoEm
            FROM public.mood_entries
            WHERE user_id = @UserId
              AND EXTRACT(MONTH FROM data) = @Mes
              AND EXTRACT(YEAR FROM data) = @Ano
            ORDER BY data ASC
            """;

        var entries = await connection.QueryAsync<MoodEntry>(
            new CommandDefinition(sql, new { UserId = userId, Mes = mes, Ano = ano }, cancellationToken: cancellationToken));

        return entries.AsList();
    }
}
