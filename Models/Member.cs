namespace ChatSupport.Models
{
    public class Member
    {
        public long member_id { get; set; }
        public long user_id { get; set; }
        public long room_id { get; set; }
        public Role role { get; set; }
        public DateTime joined_on { get; set; } = DateTime.UtcNow;

        public required User User { get; set; }
        public required Room Room { get; set; }
    }
    public enum Role
    {
        Admin,
        Moderator,
        Member
    }
}