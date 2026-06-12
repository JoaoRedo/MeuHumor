namespace MeuHumor.Api.DTOs;

public class MoodTypeDto
{
    public short Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Emoji { get; set; }
    public short Ordem { get; set; }
}
