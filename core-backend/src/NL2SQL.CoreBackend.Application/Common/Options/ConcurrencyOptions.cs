namespace NL2SQL.CoreBackend.Application.Common.Options;

/// <summary>
/// AI sorgu yolu için eşzamanlılık tavanları (kullanıcı başına + küresel).
/// </summary>
public sealed class ConcurrencyOptions
{
    public const string SectionName = "Concurrency";

    /// <summary>
    /// Tüm kullanıcılar için aynı anda yürütülebilecek AI+SQL akışı sayısı (Core içi global kapı).
    /// </summary>
    public int GlobalMaxConcurrentAiQueries { get; set; } = 15;

    /// <summary>
    /// Aynı kullanıcı için aynı anda izin verilen eşzamanlı istek (HTTP rate limiter partition).
    /// </summary>
    public int PerUserMaxConcurrentAiQueries { get; set; } = 2;

    /// <summary>
    /// Kullanıcı başına eşzamanlılık limiti dolduğunda bekleyebilecek ek istek sayısı.
    /// </summary>
    public int PerUserAiQueryQueueLimit { get; set; } = 4;
}
