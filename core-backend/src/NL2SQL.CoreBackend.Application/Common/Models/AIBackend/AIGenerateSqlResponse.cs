using System.Text.Json.Serialization;

namespace NL2SQL.CoreBackend.Application.Common.Models.AIBackend;

public class AIGenerateSqlResponse
{
    [JsonPropertyName("sql_query")]
    public string? SqlQuery { get; set; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }

    [JsonPropertyName("data")]
    public List<Dictionary<string, object>>? Data { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>Sözleşme v2 hata gövdesi veya açıklama metni.</summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("error_code")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("is_validated")]
    public bool? IsValidated { get; set; }

    [JsonPropertyName("impact_rows")]
    public int? ImpactRows { get; set; }
}
