namespace WebApplication1.Middleware;

public class FakeAuthMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    private int requestCount = 0;

    public async Task InvokeAsync(HttpContext context)
    {
        if (requestCount % 2 == 0)
        {
            requestCount++;
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("You are not authorized to access this resource.");
            return;
        }
        await _next(context);
    }
}