namespace MeuHumor.Api.Entities;

public class MoodEntryWithType : MoodEntry
{
    public string HumorLabel { get; set; } = string.Empty;
    public string? HumorEmoji { get; set; }
}
