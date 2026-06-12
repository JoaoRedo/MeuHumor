namespace MeuHumor.Api.DTOs;

public class MonthSummaryDto
{
    public short Mes { get; set; }
    public short Ano { get; set; }
    public decimal MediaHumor { get; set; }
    public int TotalDiasRegistrados { get; set; }
    public IReadOnlyDictionary<string, int> ContagemPorCategoria { get; set; } = new Dictionary<string, int>();
}
