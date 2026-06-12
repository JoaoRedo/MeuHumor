using System.ComponentModel.DataAnnotations;

namespace MeuHumor.Api.DTOs;

public class CreateMoodEntryDto
{
    [Required]
    [Range(1, 5, ErrorMessage = "Humor deve estar entre 1 (Muito Triste) e 5 (Muito Feliz).")]
    public short Humor { get; set; }

    public string? Observacao { get; set; }

    /// <summary>
    /// Data do registro. Se omitida, usa a data atual (UTC).
    /// </summary>
    public DateOnly? Data { get; set; }
}
