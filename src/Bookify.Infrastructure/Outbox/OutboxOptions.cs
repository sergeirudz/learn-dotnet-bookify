namespace Bookify.Infrastructure.Outbox;

public sealed class OutboxOptions
{
    public int IntervalInSeconds { get; init; }

    // How many outbox messages will be processes by the background job
    public int BatchSize { get; init; }
}
