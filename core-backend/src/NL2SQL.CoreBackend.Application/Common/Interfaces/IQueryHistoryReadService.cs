using NL2SQL.CoreBackend.Domain.Entities;

namespace NL2SQL.CoreBackend.Application.Common.Interfaces;

public interface IQueryHistoryReadService
{
    Task<(IReadOnlyList<QueryHistory> Items, int TotalCount)> GetPagedForUserAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        string? dbIdFilter,
        CancellationToken ct = default);
}
