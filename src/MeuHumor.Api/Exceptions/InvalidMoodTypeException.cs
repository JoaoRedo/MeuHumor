namespace MeuHumor.Api.Exceptions;

public class InvalidMoodTypeException : Exception
{
    public InvalidMoodTypeException(short moodTypeId)
        : base($"Tipo de humor {moodTypeId} é inválido ou está inativo.")
    {
        MoodTypeId = moodTypeId;
    }

    public short MoodTypeId { get; }
}
