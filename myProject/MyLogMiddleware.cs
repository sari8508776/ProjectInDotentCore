using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

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
    var actionDescriptor = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor>();
    var controller = actionDescriptor?.RouteValues["controller"] ?? "unknown";
    var action = actionDescriptor?.RouteValues["action"] ?? c.Request.Method;

   
    string? userName = c.User?.FindFirst("userId")?.Value 
                       ?? c.User?.Identity?.Name;


    // שליחה לתור
    var queue = c.RequestServices.GetRequiredService<LogQueue>();
    await queue.WriteLogAsync(new LogEntry(startTime, controller, action, userName, sw.ElapsedMilliseconds));
}
    // public async Task Invoke(HttpContext c)
    // {
    //     var sw = Stopwatch.StartNew();
    //     var startTime = DateTime.Now;

    //     await next.Invoke(c); 

    //     sw.Stop();

    //     // שליפת נתוני הניתוב
    //     var routeData = c.GetRouteData();
    //     var controller = routeData.Values["controller"]?.ToString() ?? "Unknown";
    //     var action = routeData.Values["action"]?.ToString() ?? "Unknown";
        
    //     // שליפת המשתמש
    //     var user = c.User?.FindFirst("userId")?.Value ?? "unknown"; 

    //     // הזרקת התור מתוך ה-RequestServices
    //     var queue = c.RequestServices.GetRequiredService<LogQueue>();
        
    //     // שליחת הלוג לתור
    //     await queue.WriteLogAsync(new LogEntry(startTime, controller, action, user, sw.ElapsedMilliseconds));
    // }
}

public static partial class MiddlewareExtensions
{
    public static IApplicationBuilder UseMyLogMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MyLogMiddleware>();
    }
}