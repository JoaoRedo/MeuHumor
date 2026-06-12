namespace MeuHumor.Api.Configuration;

public class SupabaseSettings
{
    public const string SectionName = "Supabase";

    public string Url { get; set; } = string.Empty;
    public string JwtAudience { get; set; } = "authenticated";
}
