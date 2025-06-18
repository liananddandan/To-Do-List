using System.Diagnostics;

namespace WebApplication1.Middleware;

//A middleware that records the execution time of each HTTP request
public class RequestTimeRecordMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        await _next(context);
        stopwatch.Stop();
        Console.WriteLine($"{context.Request.Path} took {stopwatch.ElapsedMilliseconds}ms");
    }
}