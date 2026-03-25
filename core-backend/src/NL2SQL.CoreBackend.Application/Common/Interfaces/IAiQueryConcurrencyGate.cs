namespace NL2SQL.CoreBackend.Application.Common.Interfaces;

/// <summary>
/// Tüm kullanıcılar için AI sorgu yolunda küresel eşzamanlılık tavanı (Semaphore).
/// </summary>
public interface IAiQueryConcurrencyGate
{
    /// <summary>
    /// Kapıda yer açılana kadar bekler; dönen nesne dispose edilince slot serbest kalır.
    /// </summary>
    ValueTask<IAsyncDisposable> AcquireAsync(CancellationToken cancellationToken = default);
}
