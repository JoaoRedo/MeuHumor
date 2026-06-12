namespace MeuHumor.Api.Entities;

public class MoodEntry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly Data { get; set; }
    public short Humor { get; set; }
    public string? Observacao { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
