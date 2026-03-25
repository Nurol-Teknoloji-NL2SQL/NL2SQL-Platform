namespace NL2SQL.CoreBackend.Application.Common.Options;

public sealed class QueryExecutionOptions
{
    public const string SectionName = "Query";

    public int DefaultDryRunLimit { get; set; } = 5;

    public int DefaultPageSize { get; set; } = 50;

    public int MaxPageSize { get; set; } = 200;

    public int CommandTimeoutSeconds { get; set; } = 60;
}
