using System.ComponentModel.DataAnnotations;

namespace MeuHumor.Api.DTOs;

public class CreateMoodEntryDto
{
    [Required]
    [Range(1, short.MaxValue, ErrorMessage = "Informe um tipo de humor válido.")]
    public short Humor { get; set; }

    public string? Observacao { get; set; }

    /// <summary>
    /// Data do registro. Se omitida, usa a data atual (UTC).
    /// </summary>
    public DateOnly? Data { get; set; }
}
