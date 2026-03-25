using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NL2SQL.CoreBackend.Application.Common.Interfaces;
using NL2SQL.CoreBackend.Application.Common.Models.AIBackend;

namespace NL2SQL.CoreBackend.Infrastructure.Services;

public sealed class AIBackendService : IAIBackendService
{
    private readonly HttpClient _http;
    private readonly ILogger<AIBackendService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public AIBackendService(HttpClient http, ILogger<AIBackendService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<AIGenerateSqlResponse> GenerateSqlAsync(
        AIGenerateSqlRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("AI Backend'e SQL üretim isteği gönderiliyor — db_id: {DbId}", request.DbId);

        var response = await _http.PostAsJsonAsync("/api/v1/generate-sql", request, JsonOptions, ct);
        var result = await response.Content.ReadFromJsonAsync<AIGenerateSqlResponse>(JsonOptions, ct);
        if (result is null)
        {
            _logger.LogWarning("AI Backend generate-sql: gövde ayrıştırılamadı, HTTP {Code}", (int)response.StatusCode);
            return new AIGenerateSqlResponse
            {
                Status = "failed",
                Error = $"AI yanıtı okunamadı (HTTP {(int)response.StatusCode})."
            };
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "AI Backend generate-sql HTTP {Code}: status={AiStatus}, error={Error}",
                (int)response.StatusCode,
                result.Status,
                result.Error ?? result.Message);
            if (string.IsNullOrWhiteSpace(result.Status))
                result.Status = "failed";
            if (string.IsNullOrEmpty(result.Error) && !string.IsNullOrEmpty(result.Message))
                result.Error = result.Message;
        }

        return result;
    }

    public async Task<AIExtractSchemaResponse> ExtractSchemaAsync(
        AIExtractSchemaRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("AI Backend'e şema çıkarma isteği gönderiliyor — db_id: {DbId}", request.DbId);

        var response = await _http.PostAsJsonAsync("/api/v1/onboard/extract", request, JsonOptions, ct);
        var body = await response.Content.ReadAsStringAsync(ct);
        AIExtractSchemaResponse? result = null;
        if (!string.IsNullOrWhiteSpace(body))
            result = JsonSerializer.Deserialize<AIExtractSchemaResponse>(body, JsonOptions);

        if (result is null)
        {
            return new AIExtractSchemaResponse
            {
                DbId = request.DbId,
                Error = $"AI yanıtı okunamadı (HTTP {(int)response.StatusCode})."
            };
        }

        if (!response.IsSuccessStatusCode)
        {
            result.Error = TryParseFastApiDetail(body)
                ?? result.Error
                ?? $"Şema çıkarma başarısız (HTTP {(int)response.StatusCode}).";
            _logger.LogWarning("AI extract HTTP {Code}: {Error}", (int)response.StatusCode, result.Error);
        }

        return result;
    }

    public async Task<AIRegisterSchemaResponse> RegisterSchemaAsync(
        AIRegisterSchemaRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("AI Backend'e şema kayıt isteği gönderiliyor — db_id: {DbId}", request.DbId);

        var response = await _http.PostAsJsonAsync("/api/v1/onboard/register", request, JsonOptions, ct);
        var body = await response.Content.ReadAsStringAsync(ct);
        AIRegisterSchemaResponse? result = null;
        if (!string.IsNullOrWhiteSpace(body))
            result = JsonSerializer.Deserialize<AIRegisterSchemaResponse>(body, JsonOptions);

        if (result is null)
        {
            return new AIRegisterSchemaResponse
            {
                DbId = request.DbId,
                Status = "error",
                Error = $"AI yanıtı okunamadı (HTTP {(int)response.StatusCode})."
            };
        }

        if (!response.IsSuccessStatusCode
            || string.Equals(result.Status, "error", StringComparison.OrdinalIgnoreCase))
        {
            result.Error = result.Message
                ?? TryParseFastApiDetail(body)
                ?? $"Şema kaydı başarısız (HTTP {(int)response.StatusCode}).";
            _logger.LogWarning("AI register HTTP {Code}: status={Status}, err={Err}",
                (int)response.StatusCode, result.Status, result.Error);
        }

        return result;
    }

    private static string? TryParseFastApiDetail(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("detail", out var d))
            {
                return d.ValueKind switch
                {
                    JsonValueKind.String => d.GetString(),
                    _ => d.ToString()
                };
            }
        }
        catch (JsonException)
        {
            // ignore
        }

        return null;
    }

    public async Task<bool> HealthCheckAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _http.GetAsync("/health", ct);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI Backend sağlık kontrolü başarısız");
            return false;
        }
    }
}
