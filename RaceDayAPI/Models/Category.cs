namespace RaceDayAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = ""; // e.g. U30, 30-40, 10K, 21K
        public int EventId { get; set; }
        public Event? Event { get; set; }
    }
}