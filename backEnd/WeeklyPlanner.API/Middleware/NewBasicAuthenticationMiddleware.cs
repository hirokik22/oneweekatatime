using System.Text;
using Microsoft.AspNetCore.Authorization;
using WeeklyPlanner.Model.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace WeeklyPlanner.API.Middleware
{
    public class NewBasicAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public NewBasicAuthenticationMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }

            string? authHeader = context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Authorization header not provided or invalid");
                return;
            }

            try
            {
                var auth = authHeader.Split(' ')[1];
                var emailAndPassword = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                var parts = emailAndPassword.Split(':');
                if (parts.Length != 2)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid Authorization header format");
                    return;
                }

                var email = parts[0];
                var password = parts[1];

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var loginRepository = scope.ServiceProvider.GetRequiredService<LoginRepository>();
                    var user = loginRepository.GetLoginByEmail(email);
                    if (user == null || user.PasswordHash != password)
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Incorrect credentials provided");
                        return;
                    }

                    var claims = new[] { new System.Security.Claims.Claim("LoginId", user.LoginId.ToString()) };
                    var identity = new System.Security.Claims.ClaimsIdentity(claims, "Basic");
                    context.User = new System.Security.Claims.ClaimsPrincipal(identity);
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync($"Error processing authentication: {ex.Message}");
            }
        }
    }

    public static class BasicAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NewBasicAuthenticationMiddleware>();
        }
    }
}