using FixtureForge.Application.Queries;
using FixtureForge.Contracts.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FixtureForge.Api.Controllers;

[ApiController]
[Route("api")]
public class QueriesController : ControllerBase
{
    private readonly IQueryService _service;

    public QueriesController(IQueryService service) => _service = service;

    [HttpPost("queries/execute")]
    public async Task<ActionResult<QueryExecutionResult>> Execute([FromBody] ExecuteQueryRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _service.ExecuteAsync(request, ct);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("connections/{connectionId:guid}/queries")]
    public async Task<ActionResult<IReadOnlyList<QueryHistoryDto>>> GetHistory(Guid connectionId, CancellationToken ct)
        => Ok(await _service.GetHistoryAsync(connectionId, ct));

    [HttpGet("queries/{id:guid}")]
    public async Task<ActionResult<QueryHistoryDto>> GetById(Guid id, CancellationToken ct)
    {
        var q = await _service.GetByIdAsync(id, ct);
        return q is null ? NotFound() : Ok(q);
    }
}