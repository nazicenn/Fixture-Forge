using FixtureForge.Application.Schema;
using FixtureForge.Contracts.Schema;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FixtureForge.Api.Controllers;

[ApiController]
[Route("api")]
public class TablesController : ControllerBase
{
    private readonly ITableService _service;
    private readonly IValidator<CreateTableRequest> _createValidator;
    private readonly IValidator<UpdateTableRequest> _updateValidator;

    public TablesController(ITableService service,
        IValidator<CreateTableRequest> createValidator,
        IValidator<UpdateTableRequest> updateValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet("projects/{projectId:guid}/tables")]
    public async Task<ActionResult<IReadOnlyList<TableDto>>> GetByProject(Guid projectId, CancellationToken ct)
        => Ok(await _service.GetTablesAsync(projectId, ct));

    [HttpGet("tables/{id:guid}")]
    public async Task<ActionResult<TableDto>> GetById(Guid id, CancellationToken ct)
    {
        var t = await _service.GetTableAsync(id, ct);
        return t is null ? NotFound() : Ok(t);
    }

    [HttpPost("projects/{projectId:guid}/tables")]
    public async Task<ActionResult<TableDto>> Create(Guid projectId, [FromBody] CreateTableRequest request, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        try
        {
            var created = await _service.CreateTableAsync(projectId, request, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("tables/{id:guid}")]
    public async Task<ActionResult<TableDto>> Update(Guid id, [FromBody] UpdateTableRequest request, CancellationToken ct)
    {
        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var updated = await _service.UpdateTableAsync(id, request, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("tables/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _service.DeleteTableAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}