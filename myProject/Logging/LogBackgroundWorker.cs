using System.IO;

namespace myProject;

public class LogBackgroundWorker : BackgroundService
{
    private readonly LogQueue logQueue;
    private readonly string filePath = "requests_log.txt";

    public LogBackgroundWorker(LogQueue logQueue)
    {
        this.logQueue = logQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // לולאה שרצה כל עוד האפליקציה פועלת
        await foreach (var entry in this.logQueue.ReadAllAsync(stoppingToken))
        {
            var logLine = $"{entry.StartTime:yyyy-MM-dd HH:mm:ss} | " +
                          $"Controller: {entry.ControllerName} | " +
                          $"Action: {entry.ActionName} | " +
                          $"User: {entry.UserName ?? "Anonymous"} | " +
                          $"Duration: {entry.DurationMs}ms{Environment.NewLine}";

            // כתיבה אסינכרונית לקובץ מבלי לחסום את ה-Threads של ה-API
            await File.AppendAllTextAsync(this.filePath, logLine, stoppingToken);
        }
    }
}