using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace CourseAdminSystem.API.Middleware;

public class BasicAuthenticationMiddleware {
    private const string USERNAME = "john.doe";
    private const string PASSWORD = "VerySecret!";

    private readonly RequestDelegate _next;

    public BasicAuthenticationMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context) {
        if (context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() != null) {
            await _next(context);
            return;
        }

        string? authHeader = context.Request.Headers["Authorization"];
        if (authHeader == null) {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Authorization Header value not provided");
            return;
        }

        var auth = authHeader.Split(' ')[1];
        var usernameAndPassword = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
        var username = usernameAndPassword.Split(':')[0];
        var password = usernameAndPassword.Split(':')[1];

        if (username == USERNAME && password == PASSWORD) {
            await _next(context);
        } else {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Incorrect credentials provided");
            return;
        }
    }
}

public static class BasicAuthenticationMiddlewareExtensions {
    public static IApplicationBuilder UseBasicAuthenticationMiddleware(this IApplicationBuilder builder) {
        return builder.UseMiddleware<BasicAuthenticationMiddleware>();
    }
}