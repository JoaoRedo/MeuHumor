namespace MeuHumor.Api.Exceptions;

public class MoodEntryNotEditableException : Exception
{
    public MoodEntryNotEditableException(DateOnly date)
        : base($"Apenas o humor do dia atual pode ser editado. O registro de {date:yyyy-MM-dd} não pode ser alterado.")
    {
        Date = date;
    }

    public DateOnly Date { get; }
}
