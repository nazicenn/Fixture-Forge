using FixtureForge.Application.Generation;
using FixtureForge.Contracts.Generation;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FixtureForge.Api.Controllers;

[ApiController]
[Route("api")]
public class GenerationJobsController : ControllerBase
{
    private readonly IGenerationService _service;
    private readonly IValidator<CreateGenerationJobRequest> _validator;
    private readonly IFixtureGenerator _fixtureGenerator;

    public GenerationJobsController(
        IGenerationService service,
        IValidator<CreateGenerationJobRequest> validator,
        IFixtureGenerator fixtureGenerator)
    {
        _service = service;
        _validator = validator;
        _fixtureGenerator = fixtureGenerator;
    }

    [HttpGet("projects/{projectId:guid}/generation-jobs")]
    public async Task<ActionResult<IReadOnlyList<GenerationJobDto>>> GetByProject(Guid projectId, CancellationToken ct)
        => Ok(await _service.GetJobsAsync(projectId, ct));

    [HttpGet("generation-jobs/{id:guid}")]
    public async Task<ActionResult<GenerationJobDto>> GetById(Guid id, CancellationToken ct)
    {
        var job = await _service.GetJobAsync(id, ct);
        return job is null ? NotFound() : Ok(job);
    }

    [HttpGet("generation-jobs/{id:guid}/tables")]
    public async Task<ActionResult<IReadOnlyList<GenerationJobTableDto>>> GetTables(Guid id, CancellationToken ct)
        => Ok(await _service.GetJobTablesAsync(id, ct));

    [HttpPost("projects/{projectId:guid}/generation-jobs")]
    public async Task<ActionResult<GenerationJobDto>> Create(Guid projectId, [FromBody] CreateGenerationJobRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
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

    [HttpPost("generation-jobs/{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        var cancelled = await _service.CancelAsync(id, ct);
        return cancelled ? NoContent() : BadRequest(new { error = "İş iptal edilebilir durumda değil." });
    }

    // ===== YENİ =====
    [HttpPost("generation-jobs/{id:guid}/run")]
    public async Task<IActionResult> Run(Guid id, CancellationToken ct)
    {
        try
        {
            await _fixtureGenerator.RunAsync(id, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("generation-jobs/{id:guid}/export")]
    public async Task<IActionResult> Export(Guid id, [FromQuery] string format = "sql", CancellationToken ct = default)
    {
        if (format != "sql")
            return BadRequest(new { error = "Desteklenmeyen format. Şu an sadece 'sql' destekleniyor." });

        var sql = await _fixtureGenerator.ExportSqlAsync(id, ct);
        if (sql is null)
            return NotFound(new { error = "İş bulunamadı veya başarılı değil." });

        var bytes = System.Text.Encoding.UTF8.GetBytes(sql);
        return File(bytes, "text/plain", $"fixtureforge-{id}.sql");
    }
}