using ChatSupport.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatSupport.Data
{
    public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
    {
        public DbSet<User> users => Set<User>();
        public DbSet<Room> rooms => Set<Room>();
        public DbSet<Member> members => Set<Member>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<Role>("role");


            modelBuilder.Entity<Room>()
                .HasKey(r => r.room_id);

            modelBuilder.Entity<User>()
                .HasKey(u => u.user_id);

            modelBuilder.Entity<Member>()
                .HasKey(m => m.member_id);

            modelBuilder.Entity<Member>()
                .HasIndex(m => new { m.user_id, m.room_id })
                .IsUnique();

            modelBuilder.Entity<Member>()
                .HasOne(m => m.User)
                .WithMany(u => u.Members)
                .HasForeignKey(m => m.user_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Member>()
                .HasOne(m => m.Room)
                .WithMany(r => r.Members)
                .HasForeignKey(m => m.room_id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

