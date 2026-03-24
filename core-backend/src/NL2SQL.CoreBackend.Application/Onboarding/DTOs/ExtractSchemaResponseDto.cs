namespace NL2SQL.CoreBackend.Application.Onboarding.DTOs;

public sealed class ExtractSchemaResponseDto
{
    public string DbId { get; set; } = string.Empty;

    public List<TableSchemaItemDto> Tables { get; set; } = [];

    public List<Dictionary<string, string>> FewShotExamples { get; set; } = [];
}
