using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NL2SQL.CoreBackend.Application.DatabaseConnections;
using NL2SQL.CoreBackend.Application.DatabaseConnections.DTOs;

namespace NL2SQL.CoreBackend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/databases")]
public class DatabasesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DatabasesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var userId = RequireUserId();
        if (userId is null) return Unauthorized();
        var result = await _mediator.Send(new ListDatabaseConnectionsQuery(userId.Value), ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var userId = RequireUserId();
        if (userId is null) return Unauthorized();
        var result = await _mediator.Send(new GetDatabaseConnectionByIdQuery(userId.Value, id), ct);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDatabaseConnectionRequest request, CancellationToken ct)
    {
        var userId = RequireUserId();
        if (userId is null) return Unauthorized();
        var result = await _mediator.Send(new CreateDatabaseConnectionCommand(userId.Value, request), ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDatabaseConnectionRequest request, CancellationToken ct)
    {
        var userId = RequireUserId();
        if (userId is null) return Unauthorized();
        var result = await _mediator.Send(new UpdateDatabaseConnectionCommand(userId.Value, id, request), ct);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var userId = RequireUserId();
        if (userId is null) return Unauthorized();
        var result = await _mediator.Send(new DeleteDatabaseConnectionCommand(userId.Value, id), ct);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("{id:guid}/test")]
    public async Task<IActionResult> Test(Guid id, CancellationToken ct)
    {
        var userId = RequireUserId();
        if (userId is null) return Unauthorized();
        var result = await _mediator.Send(new TestDatabaseConnectionCommand(userId.Value, id), ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private Guid? RequireUserId()
    {
        var v = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(v, out var id) ? id : null;
    }
}
