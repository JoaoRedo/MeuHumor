using MeuHumor.Api.DTOs;
using MeuHumor.Api.Exceptions;
using MeuHumor.Api.Extensions;
using MeuHumor.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeuHumor.Api.Controllers;

[ApiController]
[Route("api/mood")]
[Authorize]
public class MoodController : ControllerBase
{
    private readonly IMoodService _moodService;

    public MoodController(IMoodService moodService)
    {
        _moodService = moodService;
    }

    /// <summary>Lista os tipos de humor ativos (catálogo do banco).</summary>
    [HttpGet("types")]
    [ProducesResponseType(typeof(IReadOnlyList<MoodTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMoodTypes(CancellationToken cancellationToken)
    {
        var types = await _moodService.GetMoodTypesAsync(cancellationToken);
        return Ok(types);
    }

    /// <summary>Registra o humor do dia (um registro por dia).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(MoodEntryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterMood(
        [FromBody] CreateMoodEntryDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var userId = User.GetUserId();
            var result = await _moodService.RegisterMoodAsync(userId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetHistory), result);
        }
        catch (DuplicateMoodEntryException ex)
        {
            return Conflict(new { message = ex.Message, data = ex.EntryDate });
        }
        catch (InvalidMoodTypeException ex)
        {
            return BadRequest(new { message = ex.Message, humor = ex.MoodTypeId });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Retorna o registro de humor do dia atual, se existir.</summary>
    [HttpGet("today")]
    [ProducesResponseType(typeof(MoodEntryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetTodayMood(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var entry = await _moodService.GetTodayMoodAsync(userId, cancellationToken);
        return entry is null ? NoContent() : Ok(entry);
    }

    /// <summary>Atualiza o humor do dia atual (registros de dias anteriores não podem ser editados).</summary>
    [HttpPut("today")]
    [ProducesResponseType(typeof(MoodEntryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateTodayMood(
        [FromBody] UpdateMoodEntryDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var userId = User.GetUserId();
            var result = await _moodService.UpdateTodayMoodAsync(userId, dto, cancellationToken);
            return Ok(result);
        }
        catch (MoodEntryNotFoundException ex)
        {
            return NotFound(new { message = ex.Message, data = ex.Date });
        }
        catch (MoodEntryNotEditableException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message, data = ex.Date });
        }
        catch (InvalidMoodTypeException ex)
        {
            return BadRequest(new { message = ex.Message, humor = ex.MoodTypeId });
        }
    }

    /// <summary>Lista o histórico de humor do usuário autenticado (mais recente primeiro).</summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(IReadOnlyList<MoodEntryResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var history = await _moodService.GetHistoryAsync(userId, cancellationToken);
        return Ok(history);
    }

    /// <summary>Retorna resumo mensal com média e contagem por categoria de humor.</summary>
    [HttpGet("month-summary")]
    [ProducesResponseType(typeof(MonthSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMonthSummary(
        [FromQuery] short mes,
        [FromQuery] short ano,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.GetUserId();
            var summary = await _moodService.GetMonthSummaryAsync(userId, mes, ano, cancellationToken);
            return Ok(summary);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
