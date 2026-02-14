namespace MigrarDados.Migration;

internal static class MigrationConstants
{
    public const int ChannelInitialCapacity = 2000;
    public const int ConsumerCount = 4;

    public const int DefaultBatchSize = 1000;
    public const int MinBatchSize = 500;
    public const int MaxBatchSize = 5000;

    public const int FastBatchThresholdMs = 200;
    public const int SlowBatchThresholdMs = 1000;
    public const int BatchSizeStep = 500;

    public const int MaxInsertAttempts = 3;
    public const int RetryBaseDelayMs = 1000;
}
