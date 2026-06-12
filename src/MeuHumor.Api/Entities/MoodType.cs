namespace MeuHumor.Api.Entities;

public class MoodType
{
    public short Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Emoji { get; set; }
    public short Ordem { get; set; }
    public bool Ativo { get; set; }
}
