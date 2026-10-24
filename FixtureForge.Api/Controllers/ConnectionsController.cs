using FixtureForge.Application.Connections;
using FixtureForge.Contracts.Connections;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FixtureForge.Api.Controllers;

[ApiController]
[Route("api")]
public class ConnectionsController : ControllerBase
{
    private readonly IConnectionService _service;
    private readonly IValidator<CreateConnectionRequest> _createValidator;
    private readonly IValidator<UpdateConnectionRequest> _updateValidator;

    public ConnectionsController(
        IConnectionService service,
        IValidator<CreateConnectionRequest> createValidator,
        IValidator<UpdateConnectionRequest> updateValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet("projects/{projectId:guid}/connections")]
    public async Task<ActionResult<IReadOnlyList<ConnectionDto>>> GetByProject(Guid projectId, CancellationToken ct)
        => Ok(await _service.GetByProjectAsync(projectId, ct));

    [HttpGet("connections/{id:guid}")]
    public async Task<ActionResult<ConnectionDto>> GetById(Guid id, CancellationToken ct)
    {
        var c = await _service.GetByIdAsync(id, ct);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpPost("projects/{projectId:guid}/connections")]
    public async Task<ActionResult<ConnectionDto>> Create(Guid projectId, [FromBody] CreateConnectionRequest request, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        try
        {
            var created = await _service.CreateAsync(projectId, request, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("connections/{id:guid}")]
    public async Task<ActionResult<ConnectionDto>> Update(Guid id, [FromBody] UpdateConnectionRequest request, CancellationToken ct)
    {
        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var updated = await _service.UpdateAsync(id, request, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("connections/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("connections/{id:guid}/test")]
    public async Task<ActionResult<ConnectionTestResult>> Test(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _service.TestAsync(id, ct);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}