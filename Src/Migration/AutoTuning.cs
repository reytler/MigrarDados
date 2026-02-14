namespace MigrarDados.Migration;

internal sealed class AutoTuner
{
    private readonly object _gate = new();

    private int _batchSize;

    public AutoTuner(int initialBatchSize)
    {
        _batchSize = initialBatchSize;
    }

    public int CurrentBatchSize
    {
        get
        {
            lock (_gate)
            {
                return _batchSize;
            }
        }
    }

    public int AdjustAfterBatch(long durationMs)
    {
        lock (_gate)
        {
            if (durationMs < MigrationConstants.FastBatchThresholdMs)
            {
                _batchSize = Math.Min(_batchSize + MigrationConstants.BatchSizeStep, MigrationConstants.MaxBatchSize);
            }
            else if (durationMs > MigrationConstants.SlowBatchThresholdMs)
            {
                _batchSize = Math.Max(_batchSize - MigrationConstants.BatchSizeStep, MigrationConstants.MinBatchSize);
            }

            return _batchSize;
        }
    }
}
