using System.Text.Json;

namespace NL2SQL.CoreBackend.Application.Onboarding.DTOs;

public sealed class RegisterSchemaRequestDto
{
    public string DbId { get; set; } = string.Empty;

    public List<TableSchemaItemDto> Tables { get; set; } = [];

    public List<JsonElement> FewShotExamples { get; set; } = [];
}
