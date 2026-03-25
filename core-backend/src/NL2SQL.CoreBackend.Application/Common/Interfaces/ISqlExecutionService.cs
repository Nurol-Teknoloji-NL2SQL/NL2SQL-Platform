using NL2SQL.CoreBackend.Application.Common.Models;
using NL2SQL.CoreBackend.Domain.Enums;

namespace NL2SQL.CoreBackend.Application.Common.Interfaces;

public interface ISqlExecutionService
{
    Task<SqlExecutionResult> ExecuteReadOnlyAsync(
        string connectionString,
        DatabaseProvider provider,
        string sql,
        int commandTimeoutSeconds,
        CancellationToken ct = default);

    Task<bool> TestConnectionAsync(
        string connectionString,
        DatabaseProvider provider,
        CancellationToken ct = default);
}
