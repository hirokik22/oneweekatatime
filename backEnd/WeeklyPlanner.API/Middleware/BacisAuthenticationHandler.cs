using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using WeeklyPlanner.Model.Repositories;
using System.Net.Http.Headers;

namespace WeeklyPlanner.API.Middleware
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly LoginRepository _loginRepository;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            LoginRepository loginRepository)
            : base(options, logger, encoder)
        {
            _loginRepository = loginRepository ?? throw new ArgumentNullException(nameof(loginRepository));
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            try
            {
                // Log the raw Authorization header
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                Console.WriteLine($"Authorization Header: {authHeader}");

                var credentials = Encoding.UTF8
                    .GetString(Convert.FromBase64String(authHeader.Parameter ?? string.Empty))
                    .Split(':', 2);

                // Log the parsed credentials
                Console.WriteLine($"Parsed Credentials: Username = {credentials[0]}, Password = {credentials[1]}");

                if (credentials.Length != 2)
                    return AuthenticateResult.Fail("Invalid Authorization Header");

                var email = credentials[0];
                var password = credentials[1];

                // Check if the user exists
                var user = _loginRepository.GetLoginByUsername(email);
                Console.WriteLine($"User Retrieved: {user?.Email}, LoginId: {user?.LoginId}");

                if (user == null || user.PasswordHash != password)
                {
                    Console.WriteLine("Authentication failed: Invalid Username or Password");
                    return AuthenticateResult.Fail("Invalid Username or Password");
                }

                var claims = new[] { new Claim("LoginId", user.LoginId.ToString()) };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during authentication: {ex.Message}");
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
        }
    }
}