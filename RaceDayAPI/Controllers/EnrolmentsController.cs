using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrolmentsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public EnrolmentsController(AppDbContext db) => _db = db;
        private int? UserId() => HttpContext.Session.GetInt32("UserId");
        private string? Role() => HttpContext.Session.GetString("Role");

        /// <summary>Participant enrols for event category</summary>
        [HttpPost]
        public async Task<IActionResult> Enrol(Enrolment enrol)
        {
            if (UserId() == null) return Unauthorized();
            if (Role()!= "Participant") return Forbid("Only Participants can enrol");
            enrol.UserId = UserId().Value;
            enrol.EnrolledDate = DateTime.Now;
            _db.Enrolments.Add(enrol); await _db.SaveChangesAsync();
            return Ok(enrol);
        }

        /// <summary>Get my enrolments - Participant view own, Organiser view all for their events</summary>
        [HttpGet]
        public async Task<IActionResult> MyEnrolments()
        {
            if (UserId() == null) return Unauthorized();
            if (Role() == "Participant")
                return Ok(await _db.Enrolments.Where(e => e.UserId == UserId()).Include(e => e.Event).ToListAsync());
            else
                return Ok(await _db.Enrolments.Include(e => e.Event).Include(e => e.User).ToListAsync());
        }

        /// <summary>Cancel enrolment</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var enrol = await _db.Enrolments.FindAsync(id);
            if (enrol == null) return NotFound();
            if (enrol.UserId!= UserId() && Role()!= "Organiser") return Forbid();
            _db.Enrolments.Remove(enrol); await _db.SaveChangesAsync();
            return Ok("Cancelled");
        }
    }
}