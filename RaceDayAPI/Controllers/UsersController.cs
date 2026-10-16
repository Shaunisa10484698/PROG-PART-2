using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db) => _db = db;

        private bool IsLogged() => HttpContext.Session.GetInt32("UserId")!= null;

        /// <summary>View own profile (both roles)</summary>
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            if (!IsLogged()) return Unauthorized();
            var id = HttpContext.Session.GetInt32("UserId");
            var user = await _db.Users.FindAsync(id);
            return Ok(user);
        }

        /// <summary>Update own profile</summary>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileDto dto)
        {
            if (!IsLogged()) return Unauthorized();
            var id = HttpContext.Session.GetInt32("UserId");
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.FullName = dto.FullName?? user.FullName;
            user.Email = dto.Email?? user.Email;
            user.ProfilePictureUrl = dto.ProfilePictureUrl?? user.ProfilePictureUrl;
            await _db.SaveChangesAsync();
            return Ok(user);
        }
    }
    public class UpdateProfileDto { public string? FullName { get; set; } public string? Email { get; set; } public string? ProfilePictureUrl { get; set; } }
}