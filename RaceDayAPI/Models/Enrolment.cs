namespace RaceDayAPI.Models
{
    public class Enrolment
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public DateTime EnrolledDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Confirmed";
    }
}