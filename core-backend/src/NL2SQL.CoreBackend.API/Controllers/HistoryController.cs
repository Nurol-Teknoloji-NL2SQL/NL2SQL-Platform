using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NL2SQL.CoreBackend.Application.Query;

namespace NL2SQL.CoreBackend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/history")]
public class HistoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public HistoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? dbId = null,
        CancellationToken ct = default)
    {
        var userId = RequireUserId();
        if (userId is null) return Unauthorized();
        var result = await _mediator.Send(new GetQueryHistoryListQuery(userId.Value, page, pageSize, dbId), ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var userId = RequireUserId();
        if (userId is null) return Unauthorized();
        var result = await _mediator.Send(new GetQueryHistoryByIdQuery(userId.Value, id), ct);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var userId = RequireUserId();
        if (userId is null) return Unauthorized();
        var result = await _mediator.Send(new DeleteQueryHistoryCommand(userId.Value, id), ct);
        return result.Success ? Ok(result) : NotFound(result);
    }

    private Guid? RequireUserId()
    {
        var v = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(v, out var id) ? id : null;
    }
}
