using System.Threading.Channels;

namespace myProject;

public class LogQueue
{
    private readonly Channel<LogEntry> queue;

    public LogQueue()
    {
        this.queue = Channel.CreateUnbounded<LogEntry>();
    }

    public async ValueTask WriteLogAsync(LogEntry entry)
    {
        await this.queue.Writer.WriteAsync(entry);
    }

    public IAsyncEnumerable<LogEntry> ReadAllAsync(CancellationToken ct)
    {
        return this.queue.Reader.ReadAllAsync(ct);
    }
}