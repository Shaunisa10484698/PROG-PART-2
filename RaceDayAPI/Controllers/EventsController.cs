using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public EventsController(AppDbContext db) => _db = db;
        private string? Role() => HttpContext.Session.GetString("Role");
        private bool IsOrganiser() => Role() == "Organiser";

        /// <summary>Get all events - accessible to both Organiser and Participant</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _db.Events.ToListAsync());

        /// <summary>Get event by id</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) { var e = await _db.Events.FindAsync(id); return e == null? NotFound() : Ok(e); }

        /// <summary>Create event - Only Organiser</summary>
        [HttpPost]
        public async Task<IActionResult> Create(Event ev)
        {
            if (!IsOrganiser()) return Forbid();
            ev.OrganiserId = HttpContext.Session.GetInt32("UserId")?? 0;
            _db.Events.Add(ev); await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = ev.Id }, ev);
        }

        /// <summary>Update event - Only Organiser who created it</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Event ev)
        {
            if (!IsOrganiser()) return Forbid();
            var existing = await _db.Events.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Name = ev.Name; existing.Description = ev.Description;
            existing.Date = ev.Date; existing.Location = ev.Location;
            existing.Distance = ev.Distance; existing.EventType = ev.EventType;
            await _db.SaveChangesAsync(); return Ok(existing);
        }

        /// <summary>Delete event - Only Organiser</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsOrganiser()) return Forbid();
            var e = await _db.Events.FindAsync(id);
            if (e == null) return NotFound();
            _db.Events.Remove(e); await _db.SaveChangesAsync();
            return Ok("Deleted");
        }
    }
}