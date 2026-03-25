using Microsoft.EntityFrameworkCore;
using NL2SQL.CoreBackend.Application.Common.Interfaces;
using NL2SQL.CoreBackend.Domain.Entities;

namespace NL2SQL.CoreBackend.Infrastructure.Persistence;

public sealed class QueryHistoryReadService : IQueryHistoryReadService
{
    private readonly AppDbContext _db;

    public QueryHistoryReadService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<QueryHistory> Items, int TotalCount)> GetPagedForUserAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        string? dbIdFilter,
        CancellationToken ct = default)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _db.QueryHistories.AsNoTracking().Where(q => q.UserId == userId);
        if (!string.IsNullOrWhiteSpace(dbIdFilter))
            query = query.Where(q => q.DbId == dbIdFilter);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(q => q.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }
}
