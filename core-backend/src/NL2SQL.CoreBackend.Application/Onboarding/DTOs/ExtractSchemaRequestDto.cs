namespace NL2SQL.CoreBackend.Application.Onboarding.DTOs;

/// <summary>
/// Şema çıkarma isteği. <see cref="ConnectionString"/> boşsa kullanıcının kayıtlı DB bağlantısı kullanılır.
/// </summary>
public sealed class ExtractSchemaRequestDto
{
    public string DbId { get; set; } = string.Empty;

    public string? ConnectionString { get; set; }
}
