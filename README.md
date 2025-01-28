# IPRestrictionMiddleware

此專案展示如何在 ASP.NET Core 應用程式中設定 Forwarded Headers 和 IP 限制 Middleware。

## 設定 Forwarded Headers

在 `Program.cs` 中，加入以下程式碼來設定 Forwarded Headers：

```csharp
// ...existing code...
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = 2; 
});
// ...existing code...
app.UseForwardedHeaders();
// ...existing code...
```

## 設定 IP 限制 Middleware

在 `Program.cs` 中，加入以下程式碼來使用 IP 限制 Middleware：

```csharp
// ...existing code...
app.UseIPRestrictionMiddleware();
// ...existing code...
```

在 `IPRestrictionMiddleware.cs` 中，IP 限制 Middleware 的實作如下：

```csharp
// ...existing code...
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
/// 在Program.cs註冊時可直接使用 app.UseIPRestrictionMiddleware
/// </summary>
public static class IPRestrictionMiddlewareExtensions
{
    public static IApplicationBuilder UseIPRestrictionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<IPRestrictionMiddleware>();
    }
}
// ...existing code...
```

## 配置允許的 IP

在 `appsettings.json` 中，新增允許的 IP 清單：

```json
{
  "AllowedIPs": [
    "127.0.0.1",
    "192.168.1.1"
  ]
}
```

這樣就完成了 Forwarded Headers 和 IP 限制 Middleware 的設定。
