namespace IPRestrictionMiddleware.MiddleWares;

public class IPRestrictionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly List<string> _allowedIPs;

    public IPRestrictionMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _allowedIPs = configuration.GetSection("AllowedIPs").Get<List<string>>() ?? new List<string>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var remoteIP = context.Connection.RemoteIpAddress?.ToString();
        if (!_allowedIPs.Contains(remoteIP))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Access Denied");
            return;
        }

        await _next(context);
    }
}

/// <summary>
/// 在Program.cs註冊時可直接 app.UseIPRestrictionMiddleware
/// </summary>
public static class IPRestrictionMiddlewareExtensions
{
    public static IApplicationBuilder UseIPRestrictionMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<IPRestrictionMiddleware>();
    }
}