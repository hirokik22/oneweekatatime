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
        private readonly RoomieRepository _roomieRepository;
        public LoginController(LoginRepository loginRepository, RoomieRepository roomieRepository)
        {
            _loginRepository = loginRepository ?? throw new ArgumentNullException(nameof(loginRepository));
            _roomieRepository = roomieRepository ?? throw new ArgumentNullException(nameof(roomieRepository));
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public ActionResult Signup([FromBody] SignupRequest signupRequest)
        {
            if (signupRequest == null || string.IsNullOrWhiteSpace(signupRequest.Email) ||
                string.IsNullOrWhiteSpace(signupRequest.PasswordHash) || signupRequest.RoomieNames == null)
            {
                return BadRequest("Invalid signup data.");
            }

            // Create the login
            var login = new Login
            {
                Email = signupRequest.Email,
                PasswordHash = signupRequest.PasswordHash
            };

            try
            {
                if (!_loginRepository.CreateLogin(login))
                {
                    return BadRequest("Failed to create login.");
                }

                // Get the created login ID
                var createdLogin = _loginRepository.GetLoginByUsername(signupRequest.Email);
                if (createdLogin == null)
                {
                    return StatusCode(500, "Unable to retrieve created login.");
                }

                // Create roomies associated with the login
                foreach (var roomieName in signupRequest.RoomieNames)
                {
                    var roomie = new Roomie
                    {
                        roomiename = roomieName,
                        loginid = createdLogin.LoginId
                    };

                    if (!_roomieRepository.AddRoomie(roomie))
                    {
                        return StatusCode(500, $"Failed to create roomie: {roomieName}");
                    }
                }

                return Ok("Signup successful.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login([FromBody] Login credentials)
        {
            var login = _loginRepository.GetLoginByUsername(credentials.Email); // Corrected property name
            if (login == null || login.PasswordHash != credentials.PasswordHash) // Corrected property names
            {
                return Unauthorized("Invalid username or password.");
            }

            // Return success response with basic info (e.g., email)
            return Ok(new { Message = "Login successful", Email = credentials.Email }); // Corrected property name
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
        public class SignupRequest
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public List<string> RoomieNames { get; set; }
    }
}
