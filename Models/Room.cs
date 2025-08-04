namespace ChatSupport.Models
{
    public class Room
    {
        public long room_id { get; set; }
        public string? name { get; set; }
        public DateTime created_on { get; set; } = DateTime.UtcNow;
        public ICollection<Member> Members { get; set; } = new List<Member>();
    }
}