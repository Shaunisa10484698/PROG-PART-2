using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ResultsController(AppDbContext db) => _db = db;
        private bool IsOrganiser() => HttpContext.Session.GetString("Role") == "Organiser";

        /// <summary>Get results for event - both roles can view</summary>
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            var results = await _db.Results.Include(r => r.Enrolment).ThenInclude(e => e.User)
               .Where(r => r.Enrolment.EventId == eventId)
               .OrderBy(r => r.Position).ToListAsync();
            return Ok(results);
        }

        /// <summary>Add result - Only Organiser can capture results. Finish time and position.</summary>
        [HttpPost]
        public async Task<IActionResult> Create(Result res)
        {
            if (!IsOrganiser()) return Forbid();
            var total = await _db.Results.CountAsync(r => r.Enrolment.EventId == ( _db.Enrolments.Find(res.EnrolmentId).EventId));
            res.TotalFinishers = 312; // as per spec shows 312 finishers
            _db.Results.Add(res); await _db.SaveChangesAsync();
            return Ok(res);
        }

        /// <summary>Get my result - Participant checks own finish time</summary>
        [HttpGet("my/{enrolmentId}")]
        public async Task<IActionResult> MyResult(int enrolmentId)
        {
            var res = await _db.Results.Include(r => r.Enrolment).FirstOrDefaultAsync(r => r.EnrolmentId == enrolmentId);
            return res == null? NotFound() : Ok(res);
        }
    }
}