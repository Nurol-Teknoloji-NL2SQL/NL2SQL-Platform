using System.Text.Json;
using System.Text.Json.Serialization;

namespace NL2SQL.CoreBackend.Application.Common.Models.AIBackend;

public class AIRegisterSchemaRequest
{
    [JsonPropertyName("db_id")]
    public string DbId { get; set; } = string.Empty;

    [JsonPropertyName("tables")]
    public List<AITableSchema> Tables { get; set; } = [];

    /// <summary>AI backend <c>list[dict]</c> ile uyumlu serileştirme.</summary>
    [JsonPropertyName("few_shot_examples")]
    public List<JsonElement> FewShotExamples { get; set; } = [];
}
