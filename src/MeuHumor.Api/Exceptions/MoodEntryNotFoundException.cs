namespace MeuHumor.Api.Exceptions;

public class MoodEntryNotFoundException : Exception
{
    public MoodEntryNotFoundException(DateOnly date)
        : base($"Não há registro de humor para a data {date:yyyy-MM-dd}.")
    {
        Date = date;
    }

    public DateOnly Date { get; }
}
