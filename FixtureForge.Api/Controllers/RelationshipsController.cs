using FixtureForge.Application.Schema;
using FixtureForge.Contracts.Schema;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FixtureForge.Api.Controllers;

[ApiController]
[Route("api")]
public class RelationshipsController : ControllerBase
{
    private readonly IRelationshipService _service;
    private readonly IValidator<CreateRelationshipRequest> _validator;

    public RelationshipsController(IRelationshipService service, IValidator<CreateRelationshipRequest> validator)
    {
        _service = service;
        _validator = validator;
    }

    [HttpGet("projects/{projectId:guid}/relationships")]
    public async Task<ActionResult<IReadOnlyList<RelationshipDto>>> GetByProject(Guid projectId, CancellationToken ct)
        => Ok(await _service.GetByProjectAsync(projectId, ct));

    [HttpPost("projects/{projectId:guid}/relationships")]
    public async Task<ActionResult<RelationshipDto>> Create(Guid projectId, [FromBody] CreateRelationshipRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        try
        {
            var created = await _service.CreateAsync(projectId, request, ct);
            return Ok(created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("relationships/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}