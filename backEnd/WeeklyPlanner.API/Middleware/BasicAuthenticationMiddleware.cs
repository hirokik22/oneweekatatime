using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WeeklyPlanner.Model.Repositories;

namespace WeeklyPlanner.API.Middleware
{
    public class BasicAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public BasicAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            Console.WriteLine($"Authorization Header: {context.Request.Headers["Authorization"]}");
            // Resolve LoginRepository within the scope of this request
            using (var scope = serviceProvider.CreateScope())
            {
                var loginRepository = scope.ServiceProvider.GetRequiredService<LoginRepository>();

                // Perform your authentication logic here
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader != null && authHeader.StartsWith("Basic", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
                        var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
                        var credentials = decodedCredentials.Split(':', 2);

                        if (credentials.Length == 2)
                        {
                            var email = credentials[0];
                            var password = credentials[1];

                            var user = loginRepository.GetLoginByUsername(email);
                            if (user != null && user.PasswordHash == password)
                            {
                                // Authentication successful, set context.User
                                var claims = new[] { new Claim("LoginId", user.LoginId.ToString()) };
                                var identity = new ClaimsIdentity(claims, "Basic");
                                context.User = new ClaimsPrincipal(identity);
                            }
                            else
                            {
                                // Authentication failed
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                await context.Response.WriteAsync("Unauthorized: Invalid username or password");
                                return;
                            }
                        }
                        else
                        {
                            // Invalid credentials format
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            await context.Response.WriteAsync("Bad Request: Invalid Authorization header format");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle decoding errors
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync($"Bad Request: Invalid Authorization header. {ex.Message}");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }

    // Extension method for middleware registration
    public static class BasicAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app)
        {
            return app.UseMiddleware<BasicAuthenticationMiddleware>();
        }
    }
}