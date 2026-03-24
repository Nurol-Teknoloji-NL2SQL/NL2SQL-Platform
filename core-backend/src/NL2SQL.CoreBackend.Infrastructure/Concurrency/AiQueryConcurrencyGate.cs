using Microsoft.Extensions.Options;
using NL2SQL.CoreBackend.Application.Common.Interfaces;
using NL2SQL.CoreBackend.Application.Common.Options;

namespace NL2SQL.CoreBackend.Infrastructure.Concurrency;

public sealed class AiQueryConcurrencyGate : IAiQueryConcurrencyGate
{
    private readonly SemaphoreSlim _semaphore;

    public AiQueryConcurrencyGate(IOptions<ConcurrencyOptions> options)
    {
        var n = Math.Max(1, options.Value.GlobalMaxConcurrentAiQueries);
        _semaphore = new SemaphoreSlim(n, n);
    }

    public async ValueTask<IAsyncDisposable> AcquireAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        return new ReleaseDisposable(_semaphore);
    }

    private sealed class ReleaseDisposable : IAsyncDisposable
    {
        private readonly SemaphoreSlim _semaphore;

        public ReleaseDisposable(SemaphoreSlim semaphore) => _semaphore = semaphore;

        public ValueTask DisposeAsync()
        {
            _semaphore.Release();
            return ValueTask.CompletedTask;
        }
    }
}
