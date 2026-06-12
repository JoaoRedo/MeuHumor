namespace MeuHumor.Api.Entities;

public enum MoodLevel : short
{
    MuitoTriste = 1,
    Triste = 2,
    Neutro = 3,
    Feliz = 4,
    MuitoFeliz = 5
}

public static class MoodLevelExtensions
{
    public static string ToLabel(this MoodLevel level) => level switch
    {
        MoodLevel.MuitoTriste => "Muito Triste",
        MoodLevel.Triste => "Triste",
        MoodLevel.Neutro => "Neutro",
        MoodLevel.Feliz => "Feliz",
        MoodLevel.MuitoFeliz => "Muito Feliz",
        _ => "Desconhecido"
    };
}
