namespace NL2SQL.CoreBackend.Application.Onboarding.DTOs;

public sealed class RegisterSchemaResponseDto
{
    public string Status { get; set; } = string.Empty;

    public string DbId { get; set; } = string.Empty;

    public int ChunksSaved { get; set; }
}
