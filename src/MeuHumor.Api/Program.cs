using Dapper;
using MeuHumor.Api.Configuration;
using MeuHumor.Api.Data;
using MeuHumor.Api.Extensions;
using Npgsql;

SqlMapper.AddTypeHandler(new DapperDateOnlyHandler());

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddMeuHumorServices(builder.Configuration);
builder.Services.AddSupabaseAuthentication(builder.Configuration);
builder.Services.AddMeuHumorCors(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();

    var connectionString = builder.Configuration
        .GetSection(DatabaseSettings.SectionName)
        .Get<DatabaseSettings>()?.ConnectionString;

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        app.Logger.LogWarning("Database:ConnectionString não configurada em appsettings.Development.json");
    }
    else
    {
        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            app.Logger.LogInformation("Conexão com PostgreSQL estabelecida com sucesso.");

            await using var cmd = new NpgsqlCommand(
                "SELECT to_regclass('public.mood_entries'), to_regclass('public.mood_types')", connection);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                if (reader.IsDBNull(0))
                    app.Logger.LogWarning("Tabela public.mood_entries não encontrada. Execute database/schema.sql no Supabase.");
                if (reader.IsDBNull(1))
                    app.Logger.LogWarning("Tabela public.mood_types não encontrada. Execute database/migrations/003_create_mood_types.sql no Supabase.");
            }
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Falha ao conectar no PostgreSQL. Verifique a connection string.");
        }
    }
}

app.UseCors("Frontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
