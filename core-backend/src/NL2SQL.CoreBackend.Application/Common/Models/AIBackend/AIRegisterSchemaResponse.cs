using System.Text.Json.Serialization;

namespace NL2SQL.CoreBackend.Application.Common.Models.AIBackend;

public class AIRegisterSchemaResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("db_id")]
    public string DbId { get; set; } = string.Empty;

    [JsonPropertyName("chunks_saved")]
    public int ChunksSaved { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>HTTP hatası veya boş gövde durumunda handler tarafından doldurulur.</summary>
    public string? Error { get; set; }
}
