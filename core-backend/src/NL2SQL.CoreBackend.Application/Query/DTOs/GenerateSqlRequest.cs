namespace NL2SQL.CoreBackend.Application.Query.DTOs;

public record GenerateSqlRequest(
    string Query,
    string DbId,
    int? DryRunLimit = null,
    int Page = 1,
    int PageSize = 0
);
