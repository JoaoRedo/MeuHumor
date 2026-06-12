using System.ComponentModel.DataAnnotations;

namespace MeuHumor.Api.DTOs;

public class UpdateMoodEntryDto
{
    [Required]
    [Range(1, short.MaxValue, ErrorMessage = "Informe um tipo de humor válido.")]
    public short Humor { get; set; }

    public string? Observacao { get; set; }
}
