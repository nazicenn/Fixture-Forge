using FixtureForge.Application.Schema;
using FixtureForge.Contracts.Schema;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FixtureForge.Api.Controllers;

[ApiController]
[Route("api")]
public class ColumnsController : ControllerBase
{
    private readonly ITableService _service;
    private readonly IValidator<CreateColumnRequest> _createValidator;
    private readonly IValidator<UpdateColumnRequest> _updateValidator;

    public ColumnsController(ITableService service,
        IValidator<CreateColumnRequest> createValidator,
        IValidator<UpdateColumnRequest> updateValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet("tables/{tableId:guid}/columns")]
    public async Task<ActionResult<IReadOnlyList<ColumnDto>>> GetByTable(Guid tableId, CancellationToken ct)
        => Ok(await _service.GetColumnsAsync(tableId, ct));

    [HttpGet("columns/{id:guid}")]
    public async Task<ActionResult<ColumnDto>> GetById(Guid id, CancellationToken ct)
    {
        var c = await _service.GetColumnAsync(id, ct);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpPost("tables/{tableId:guid}/columns")]
    public async Task<ActionResult<ColumnDto>> Create(Guid tableId, [FromBody] CreateColumnRequest request, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        try
        {
            var created = await _service.CreateColumnAsync(tableId, request, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("columns/{id:guid}")]
    public async Task<ActionResult<ColumnDto>> Update(Guid id, [FromBody] UpdateColumnRequest request, CancellationToken ct)
    {
        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        try
        {
            var updated = await _service.UpdateColumnAsync(id, request, ct);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("columns/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _service.DeleteColumnAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}