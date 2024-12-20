using Microsoft.AspNetCore.Mvc;
using WeeklyPlanner.Model.Entities;
using WeeklyPlanner.Model.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace WeeklyPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly LoginRepository _loginRepository;

        public LoginController(LoginRepository loginRepository)
        {
            _loginRepository = loginRepository ?? throw new ArgumentNullException(nameof(loginRepository));
        }
                [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login([FromBody] Login credentials)
        {
            var login = _loginRepository.GetLoginByUsername(credentials.Username);
            if (login == null || login.Password != credentials.Password)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Return success response with basic info (e.g., username)
            return Ok(new { Message = "Login successful", Username = credentials.Username });
        }

        [HttpGet("status")]
        public ActionResult<string> Status()
        {
            return Ok("Login system is operational.");
        }

        // GET: api/login
        [HttpGet]
        public ActionResult<IEnumerable<Login>> GetAllLogins()
        {
            try
            {
                var logins = _loginRepository.GetAllLogins();
                return Ok(logins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/login/{loginid}
        [HttpGet("{loginid}")]
        public ActionResult<Login> GetLoginById([FromRoute] int loginid)
        {
            try
            {
                var login = _loginRepository.GetLoginById(loginid);
                if (login == null)
                {
                    return NotFound($"Login with ID {loginid} not found.");
                }

                return Ok(login);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/login
        [HttpPost]
        public ActionResult CreateLogin([FromBody] Login login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = _loginRepository.CreateLogin(login);
                if (result)
                {
                    return Ok("Login created successfully.");
                }

                return BadRequest("Failed to create login.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/login/{loginid}
        [HttpPut("{loginid}")]
        public ActionResult UpdateLogin([FromRoute] int loginid, [FromBody] Login login)
        {
            if (!ModelState.IsValid || login == null || loginid != login.LoginId)
            {
                return BadRequest("Invalid login data or ID mismatch.");
            }

            try
            {
                var result = _loginRepository.UpdateLogin(login);
                if (result)
                {
                    return Ok("Login updated successfully.");
                }

                return BadRequest("Failed to update login.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/login/{loginid}
        [HttpDelete("{loginid}")]
        public ActionResult DeleteLogin([FromRoute] int loginid)
        {
            try
            {
                var result = _loginRepository.DeleteLogin(loginid);
                if (result)
                {
                    return NoContent();
                }

                return BadRequest("Failed to delete login.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
