namespace NL2SQL.CoreBackend.Application.Onboarding.DTOs;

public sealed class TableSchemaItemDto
{
    public string Name { get; set; } = string.Empty;

    public List<string> Columns { get; set; } = [];

    public string HumanDescription { get; set; } = "";

    public string BusinessRules { get; set; } = "";
}
