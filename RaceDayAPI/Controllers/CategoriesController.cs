using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CategoriesController(AppDbContext db) => _db = db;
        private bool IsOrganiser() => HttpContext.Session.GetString("Role") == "Organiser";

        /// <summary>Get categories for an event</summary>
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetByEvent(int eventId) => Ok(await _db.Categories.Where(c => c.EventId == eventId).ToListAsync());

        /// <summary>Create category for event - Only Organiser</summary>
        [HttpPost]
        public async Task<IActionResult> Create(Category cat)
        {
            if (!IsOrganiser()) return Forbid();
            _db.Categories.Add(cat); await _db.SaveChangesAsync();
            return Ok(cat);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsOrganiser()) return Forbid();
            var c = await _db.Categories.FindAsync(id);
            if (c == null) return NotFound();
            _db.Categories.Remove(c); await _db.SaveChangesAsync();
            return Ok("Deleted");
        }
    }
}