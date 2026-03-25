using MediatR;
using NL2SQL.CoreBackend.Application.Common.Interfaces;
using NL2SQL.CoreBackend.Application.Common.Models;
using NL2SQL.CoreBackend.Application.Query.DTOs;
using NL2SQL.CoreBackend.Domain.Entities;

namespace NL2SQL.CoreBackend.Application.Query;

public record GetQueryHistoryListQuery(Guid UserId, int Page, int PageSize, string? DbId)
    : IRequest<ApiResponse<PaginatedResponse<QueryHistoryDto>>>;

public record GetQueryHistoryByIdQuery(Guid UserId, Guid Id) : IRequest<ApiResponse<QueryHistoryDto>>;

public record DeleteQueryHistoryCommand(Guid UserId, Guid Id) : IRequest<ApiResponse<object?>>;

public sealed class QueryHistoryHandlers :
    IRequestHandler<GetQueryHistoryListQuery, ApiResponse<PaginatedResponse<QueryHistoryDto>>>,
    IRequestHandler<GetQueryHistoryByIdQuery, ApiResponse<QueryHistoryDto>>,
    IRequestHandler<DeleteQueryHistoryCommand, ApiResponse<object?>>
{
    private readonly IUnitOfWork _uow;
    private readonly IQueryHistoryReadService _read;

    public QueryHistoryHandlers(IUnitOfWork uow, IQueryHistoryReadService read)
    {
        _uow = uow;
        _read = read;
    }

    public async Task<ApiResponse<PaginatedResponse<QueryHistoryDto>>> Handle(GetQueryHistoryListQuery q, CancellationToken ct)
    {
        var (items, total) = await _read.GetPagedForUserAsync(q.UserId, q.Page, q.PageSize, q.DbId, ct);
        var dtos = items.Select(Map).ToList();
        var paginated = new PaginatedResponse<QueryHistoryDto>
        {
            Items = dtos,
            PageNumber = Math.Max(1, q.Page),
            PageSize = q.PageSize,
            TotalCount = total
        };
        return ApiResponse<PaginatedResponse<QueryHistoryDto>>.Ok(paginated);
    }

    public async Task<ApiResponse<QueryHistoryDto>> Handle(GetQueryHistoryByIdQuery q, CancellationToken ct)
    {
        var h = await _uow.QueryHistories.GetByIdAsync(q.Id, ct);
        if (h is null || h.UserId != q.UserId)
            return ApiResponse<QueryHistoryDto>.Fail("Kayıt bulunamadı.");
        return ApiResponse<QueryHistoryDto>.Ok(Map(h));
    }

    public async Task<ApiResponse<object?>> Handle(DeleteQueryHistoryCommand cmd, CancellationToken ct)
    {
        var h = await _uow.QueryHistories.GetByIdAsync(cmd.Id, ct);
        if (h is null || h.UserId != cmd.UserId)
            return ApiResponse<object?>.Fail("Kayıt bulunamadı.");

        _uow.QueryHistories.Remove(h);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<object?>.Ok(null, "Silindi.");
    }

    private static QueryHistoryDto Map(QueryHistory h) => new(
        h.Id,
        h.DbId,
        h.NaturalLanguageQuery,
        h.GeneratedSql,
        h.Explanation,
        h.ExecutionStatus.ToString(),
        h.ExecutionTimeMs,
        h.ErrorMessage,
        h.CreatedAt);
}
