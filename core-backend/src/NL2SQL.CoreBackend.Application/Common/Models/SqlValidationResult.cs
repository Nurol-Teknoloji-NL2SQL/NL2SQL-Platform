namespace NL2SQL.CoreBackend.Application.Common.Models;

public sealed record SqlValidationResult(bool IsValid, string? ErrorCode, string? Message);
