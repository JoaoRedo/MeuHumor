namespace MeuHumor.Api.Entities;

public class Report
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public short Mes { get; set; }
    public short Ano { get; set; }
    public decimal? MediaHumor { get; set; }
    public string? ResumoJson { get; set; }
    public bool EnviadoEmail { get; set; }
    public bool EnviadoWhatsapp { get; set; }
    public DateTime? EnviadoEm { get; set; }
    public DateTime CriadoEm { get; set; }
}
