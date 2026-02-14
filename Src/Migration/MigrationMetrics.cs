using System.Diagnostics;

namespace MigrarDados.Migration;

internal sealed class MigrationMetrics
{
    private long _totalRead;
    private long _totalInserted;
    private long _totalInsertMs;

    public Stopwatch TotalStopwatch { get; } = Stopwatch.StartNew();
    public Stopwatch ReadStopwatch { get; } = new();

    public void AddRead(long count = 1) => Interlocked.Add(ref _totalRead, count);

    public long TotalRead => Interlocked.Read(ref _totalRead);

    public long AddInserted(long count) => Interlocked.Add(ref _totalInserted, count);

    public long TotalInserted => Interlocked.Read(ref _totalInserted);

    public void AddInsertMilliseconds(long elapsedMs) => Interlocked.Add(ref _totalInsertMs, elapsedMs);

    public TimeSpan TotalInsertDuration => TimeSpan.FromMilliseconds(Interlocked.Read(ref _totalInsertMs));
}
