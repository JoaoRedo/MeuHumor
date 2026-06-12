using MeuHumor.Api.Configuration;
using MeuHumor.Api.Repositories;
using MeuHumor.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MeuHumor.Api.Extensions;

public static class ServiceCollectionExtensions
{
    private static readonly string[] SupportedAlgorithms =
    [
        SecurityAlgorithms.EcdsaSha256,
        SecurityAlgorithms.RsaSha256,
        SecurityAlgorithms.HmacSha256,
    ];
    public static IServiceCollection AddMeuHumorServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.SectionName));
        services.Configure<SupabaseSettings>(configuration.GetSection(SupabaseSettings.SectionName));

        services.AddMemoryCache();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMoodTypeRepository, MoodTypeRepository>();
        services.AddScoped<IMoodEntryRepository, MoodEntryRepository>();
        services.AddScoped<IMoodService, MoodService>();

        return services;
    }

    public static IServiceCollection AddSupabaseAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var supabaseSettings = configuration.GetSection(SupabaseSettings.SectionName).Get<SupabaseSettings>()
            ?? throw new InvalidOperationException("Configuração Supabase não encontrada.");

        if (string.IsNullOrWhiteSpace(supabaseSettings.Url))
            throw new InvalidOperationException("Supabase:Url é obrigatório.");

        var issuer = $"{supabaseSettings.Url.TrimEnd('/')}/auth/v1";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Projetos Supabase atuais usam chaves assimétricas (ES256) via JWKS.
                options.MetadataAddress = $"{issuer}/.well-known/openid-configuration";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = supabaseSettings.JwtAudience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    NameClaimType = "sub",
                    ClockSkew = TimeSpan.FromMinutes(2),
                    ValidAlgorithms = SupportedAlgorithms,
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddMeuHumorCors(this IServiceCollection services, IConfiguration configuration)
    {
        var corsSettings = configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>() ?? new CorsSettings();
        var origins = corsSettings.AllowedOrigins.Length > 0
            ? corsSettings.AllowedOrigins
            : ["http://localhost:5173"];

        services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                policy.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}
