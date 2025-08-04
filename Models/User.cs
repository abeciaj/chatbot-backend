namespace ChatSupport.Models
{
    public class User
    {
        public long user_id { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public DateTime created_on { get; set; }
        public ICollection<Member> Members { get; set; } = new List<Member>();
    }
}
