using MediatR;
using NL2SQL.CoreBackend.Application.Common.Interfaces;
using NL2SQL.CoreBackend.Application.Common.Models;
using NL2SQL.CoreBackend.Application.Onboarding.DTOs;

namespace NL2SQL.CoreBackend.Application.Onboarding;

public record GetCachedSchemaQuery(Guid UserId, string DbId) : IRequest<ApiResponse<CachedSchemaResponse>>;

public sealed class GetCachedSchemaQueryHandler : IRequestHandler<GetCachedSchemaQuery, ApiResponse<CachedSchemaResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetCachedSchemaQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ApiResponse<CachedSchemaResponse>> Handle(GetCachedSchemaQuery q, CancellationToken ct)
    {
        var owns = await _uow.DatabaseConnections.AnyAsync(
            c => c.UserId == q.UserId && c.DbId == q.DbId, ct);
        if (!owns)
            return ApiResponse<CachedSchemaResponse>.Fail("Bu veritabanı için yetkiniz yok veya kayıt bulunamadı.");

        var caches = await _uow.SchemaCaches.FindAsync(s => s.DbId == q.DbId, ct);
        var cache = caches.OrderByDescending(s => s.UpdatedAt).FirstOrDefault();
        if (cache is null)
            return ApiResponse<CachedSchemaResponse>.Fail("Önbellekte şema bulunamadı.");

        if (cache.ExpiresAt < DateTime.UtcNow)
            return ApiResponse<CachedSchemaResponse>.Fail("Şema önbelleği süresi dolmuş. Lütfen şemayı yeniden yükleyin.");

        var dto = new CachedSchemaResponse(
            cache.DbId,
            cache.DatabaseName,
            cache.SchemaJson,
            cache.ExpiresAt,
            cache.UpdatedAt);

        return ApiResponse<CachedSchemaResponse>.Ok(dto);
    }
}
