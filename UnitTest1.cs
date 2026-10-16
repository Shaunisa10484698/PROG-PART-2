using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using RaceDayAPI.Helpers;

namespace RaceDayAPI.Tests
{
    public class RaceDayTests
    {
        private AppDbContext GetDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public void PasswordHasher_HashesCorrectly()
        {
            var hash = PasswordHasher.Hash("test123");
            Assert.True(PasswordHasher.Verify("test123", hash));
        }

        [Fact]
        public async Task Can_Create_Event()
        {
            var db = GetDb();
            var ev = new Event { Name = "Comrades", Location = "Durban", Date = DateTime.Now.AddMonths(1), Distance = 21, EventType = "Run", OrganiserId = 1 };
            db.Events.Add(ev);
            await db.SaveChangesAsync();
            Assert.Equal(1, await db.Events.CountAsync());
        }

        [Fact]
        public async Task Can_Enrol_Participant()
        {
            var db = GetDb();
            var user = new User { Username = "john", PasswordHash = PasswordHasher.Hash("123"), Role = "Participant", FullName = "John", Email = "john@test.com" };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            var enrol = new Enrolment { EventId = 1, UserId = user.Id, CategoryId = 1 };
            db.Enrolments.Add(enrol);
            await db.SaveChangesAsync();
            Assert.Equal(1, await db.Enrolments.CountAsync());
        }

        [Fact]
        public async Task Can_Add_Result_With_312_Finishers()
        {
            var db = GetDb();
            var result = new Result { EnrolmentId = 1, FinishTime = TimeSpan.FromHours(2), Position = 1, TotalFinishers = 312 };
            db.Results.Add(result);
            await db.SaveChangesAsync();
            var saved = await db.Results.FirstAsync();
            Assert.Equal(312, saved.TotalFinishers);
        }
    }
}