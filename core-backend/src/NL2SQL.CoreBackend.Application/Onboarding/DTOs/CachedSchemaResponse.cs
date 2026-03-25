namespace NL2SQL.CoreBackend.Application.Onboarding.DTOs;

public record CachedSchemaResponse(
    string DbId,
    string DatabaseName,
    string SchemaJson,
    DateTime ExpiresAt,
    DateTime UpdatedAt);
