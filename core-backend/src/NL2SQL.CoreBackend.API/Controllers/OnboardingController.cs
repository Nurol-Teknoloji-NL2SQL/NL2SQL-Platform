using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NL2SQL.CoreBackend.Application.Onboarding;
using NL2SQL.CoreBackend.Application.Onboarding.DTOs;

namespace NL2SQL.CoreBackend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/onboarding")]
public class OnboardingController : ControllerBase
{
    private readonly IMediator _mediator;

    public OnboardingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Canlı veritabanından şemayı çıkarır (AI Backend).
    /// </summary>
    [HttpPost("extract")]
    [EnableRateLimiting("onboarding")]
    public async Task<IActionResult> Extract([FromBody] ExtractSchemaRequestDto request, CancellationToken ct)
    {
        var userId = RequireUserId();
        if (userId is null) return Unauthorized();
        var result = await _mediator.Send(new ExtractSchemaCommand(userId.Value, request), ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Zenginleştirilmiş şemayı vektör deposuna kaydeder (AI Backend) ve Core önbelleğini günceller.
    /// </summary>
    [HttpPost("register")]
    [EnableRateLimiting("onboarding")]
    public async Task<IActionResult> Register([FromBody] RegisterSchemaRequestDto request, CancellationToken ct)
    {
        var userId = RequireUserId();
        if (userId is null) return Unauthorized();
        var result = await _mediator.Send(new RegisterSchemaCommand(userId.Value, request), ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// PostgreSQL'de önbelleğe alınmış şemayı döndürür (AI extract/register sonrası doldurulur).
    /// </summary>
    [HttpGet("schema/{dbId}")]
    public async Task<IActionResult> GetCachedSchema(string dbId, CancellationToken ct)
    {
        var userId = RequireUserId();
        if (userId is null) return Unauthorized();
        var result = await _mediator.Send(new GetCachedSchemaQuery(userId.Value, dbId), ct);
        return result.Success ? Ok(result) : NotFound(result);
    }

    private Guid? RequireUserId()
    {
        var v = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(v, out var id) ? id : null;
    }
}
