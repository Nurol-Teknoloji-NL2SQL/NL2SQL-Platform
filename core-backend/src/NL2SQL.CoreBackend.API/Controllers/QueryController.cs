using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NL2SQL.CoreBackend.Application.Query.DTOs;
using NL2SQL.CoreBackend.Application.Query.GenerateSql;

namespace NL2SQL.CoreBackend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/query")]
[EnableRateLimiting("ai-query")]
public class QueryController : ControllerBase
{
    private readonly IMediator _mediator;

    public QueryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Doğal dil sorusunu AI Backend üzerinden SQL'e çevirir; Core ikincil doğrulama ve salt okunur çalıştırma yapar.
    /// </summary>
    [HttpPost("generate-sql")]
    public async Task<IActionResult> GenerateSql([FromBody] GenerateSqlRequest request, CancellationToken ct)
    {
        var userId = RequireUserId();
        if (userId is null)
            return Unauthorized();

        var result = await _mediator.Send(new GenerateSqlCommand(userId.Value, request), ct);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    private Guid? RequireUserId()
    {
        var v = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(v, out var id) ? id : null;
    }
}
