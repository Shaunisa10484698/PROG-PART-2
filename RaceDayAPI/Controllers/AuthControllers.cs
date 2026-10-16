using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Helpers;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AuthController(AppDbContext db) => _db = db;

        /// <summary>Register new user as Organiser or Participant. Password will be hashed.</summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (await _db.Users.AnyAsync(u => u.Username == user.Username))
                return BadRequest("Username already exists");
            user.PasswordHash = PasswordHasher.Hash(user.PasswordHash);
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Ok(new { user.Id, user.Username, user.Role });
        }

        /// <summary>Login with username/password. Creates session for role-based access.</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null ||!PasswordHasher.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Role", user.Role);
            return Ok(new { message = "Logged in", user.Id, user.Role });
        }

        [HttpPost("logout")]
        public IActionResult Logout() { HttpContext.Session.Clear(); return Ok("Logged out"); }
    }
    public class LoginDto { public string Username { get; set; } = ""; public string Password { get; set; } = ""; }
}