namespace MeuHumor.Api.Exceptions;

public class DuplicateMoodEntryException : Exception
{
    public DuplicateMoodEntryException(DateOnly entryDate)
        : base($"Já existe um registro de humor para a data {entryDate:yyyy-MM-dd}.")
    {
        EntryDate = entryDate;
    }

    public DateOnly EntryDate { get; }
}
