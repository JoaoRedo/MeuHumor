namespace MeuHumor.Api.DTOs;

public class MoodEntryResponseDto
{
    public Guid Id { get; set; }
    public DateOnly Data { get; set; }
    public short Humor { get; set; }
    public string HumorLabel { get; set; } = string.Empty;
    public string? HumorEmoji { get; set; }
    public string? Observacao { get; set; }
    public DateTime CriadoEm { get; set; }
}
