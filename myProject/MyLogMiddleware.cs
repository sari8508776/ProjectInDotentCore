using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace myProject;

public class MyLogMiddleware
{
    private readonly RequestDelegate next;

    public MyLogMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext c)
    {
        var sw = Stopwatch.StartNew();
        var startTime = DateTime.Now;

        await next.Invoke(c); 

        sw.Stop();

        var endpoint = c.GetEndpoint();
        var routeValues = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor>()?.RouteValues;
        
        var controller = routeValues?["controller"]?.ToString() ?? "StaticFile";
        var action = routeValues?["action"]?.ToString() ?? c.Request.Method;
        var userName = c.User?.Identity?.Name 
                        ?? c.User?.FindFirst("username")?.Value 
                        ?? "Guest";

        var queue = c.RequestServices.GetRequiredService<LogQueue>();
        await queue.WriteLogAsync(new LogEntry(startTime, controller!, action!, userName, sw.ElapsedMilliseconds));
    }
}

// זה החלק שהיה חסר או כפול וגרם לשגיאה
public static partial class MiddlewareExtensions
{
    public static IApplicationBuilder UseMyLogMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MyLogMiddleware>();
    }
}