using FixtureForge.Application.Projects;
using FixtureForge.Contracts.Projects;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FixtureForge.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _service;
    private readonly IValidator<CreateProjectRequest> _createValidator;
    private readonly IValidator<UpdateProjectRequest> _updateValidator;

    public ProjectsController(
        IProjectService service,
        IValidator<CreateProjectRequest> createValidator,
        IValidator<UpdateProjectRequest> updateValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id, CancellationToken ct)
    {
        var project = await _service.GetByIdAsync(id, ct);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectRequest request, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var created = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> Update(Guid id, [FromBody] UpdateProjectRequest request, CancellationToken ct)
    {
        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var updated = await _service.UpdateAsync(id, request, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}