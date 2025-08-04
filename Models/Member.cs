namespace ChatSupport.Models
{
    public class Member
    {
        public long member_id { get; set; }
        public long user_id { get; set; }
        public long room_id { get; set; }
        public int role { get; set; } // 0 = Moderator, 1 = Admin, 2 = Member
        public DateTime joined_on { get; set; } = DateTime.UtcNow;

        public required User User { get; set; }
        public required Room Room { get; set; }
    }

}