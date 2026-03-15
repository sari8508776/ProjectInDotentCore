namespace myProject;

public record LogEntry(
    DateTime StartTime,
    string ControllerName,
    string ActionName,
    string? UserName,
    long DurationMs
);