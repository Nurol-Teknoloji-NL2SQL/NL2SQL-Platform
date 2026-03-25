namespace NL2SQL.CoreBackend.Application.Common.Models;

public sealed record SqlExecutionResult(
    bool Success,
    IReadOnlyList<Dictionary<string, object?>>? Rows,
    string? ErrorMessage,
    int RowCount,
    long ElapsedMs);
