namespace MeuHumor.Api;

public static class AppTimeZone
{
    private const string DefaultTimeZoneId = "America/Sao_Paulo";
    private const string WindowsTimeZoneId = "E. South America Standard Time";

    public static TimeZoneInfo Resolve()
    {
        var configured = Environment.GetEnvironmentVariable("TZ");
        if (!string.IsNullOrWhiteSpace(configured))
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(configured);
            }
            catch (TimeZoneNotFoundException)
            {
                // segue para os ids padrão
            }
        }

        foreach (var id in new[] { DefaultTimeZoneId, WindowsTimeZoneId })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
                // tenta o próximo id
            }
        }

        throw new InvalidOperationException(
            $"Fuso horário '{DefaultTimeZoneId}' não encontrado. Instale o pacote tzdata no container.");
    }

    public static DateOnly GetTodayDate()
    {
        var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Resolve());
        return DateOnly.FromDateTime(now);
    }
}
