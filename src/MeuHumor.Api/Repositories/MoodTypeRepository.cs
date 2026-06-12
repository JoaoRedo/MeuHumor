using Dapper;
using MeuHumor.Api.Configuration;
using MeuHumor.Api.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Npgsql;

namespace MeuHumor.Api.Repositories;

public class MoodTypeRepository : IMoodTypeRepository
{
    private const string ActiveCacheKey = "mood_types_active";
    private const string AllCacheKey = "mood_types_all";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private readonly string _connectionString;
    private readonly IMemoryCache _cache;

    public MoodTypeRepository(IOptions<DatabaseSettings> databaseSettings, IMemoryCache cache)
    {
        _connectionString = databaseSettings.Value.ConnectionString;
        _cache = cache;
    }

    public Task<IReadOnlyList<MoodType>> GetActiveAsync(CancellationToken cancellationToken = default) =>
        _cache.GetOrCreateAsync(ActiveCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await QueryAsync(activeOnly: true, cancellationToken);
        })!;

    public Task<IReadOnlyList<MoodType>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _cache.GetOrCreateAsync(AllCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await QueryAsync(activeOnly: false, cancellationToken);
        })!;

    public async Task<MoodType?> GetActiveByIdAsync(short id, CancellationToken cancellationToken = default)
    {
        var types = await GetActiveAsync(cancellationToken);
        return types.FirstOrDefault(t => t.Id == id);
    }

    private async Task<IReadOnlyList<MoodType>> QueryAsync(bool activeOnly, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        var sql = """
            SELECT id     AS Id,
                   label  AS Label,
                   emoji  AS Emoji,
                   ordem  AS Ordem,
                   ativo  AS Ativo
            FROM public.mood_types
            """;

        if (activeOnly)
            sql += " WHERE ativo = TRUE";

        sql += " ORDER BY ordem ASC, id ASC";

        var types = await connection.QueryAsync<MoodType>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return types.AsList();
    }
}
