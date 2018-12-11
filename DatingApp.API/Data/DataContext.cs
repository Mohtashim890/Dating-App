using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        // to Make complete duplicate of DbContext, Pass DbContextoptions to
        // DataContext Constructor
        public DataContext(DbContextOptions<DataContext> options) : base (options){}

        public DbSet<Value> Values { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Photo> Photos { get; set; }

        public DbSet<Like> Likes {get; set; }
        public DbSet<Message> Messages { get; set; }

        // configure tables
        protected override void OnModelCreating(ModelBuilder builder) 
        {
            // configure primary key (combination of LikerId and LikeeId) of table
            builder.Entity<Like>().HasKey(k => new {k.LikerId, k.LikeeId});
            // define relationship one likee--->many likers
            builder.Entity<Like>()
                .HasOne(u => u.Likee)
                .WithMany(u => u.Likers)
                .HasForeignKey(u => u.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);
            // define relationship one liker--->many likees
            builder.Entity<Like>()
                .HasOne(u => u.Liker)
                .WithMany(u => u.Likees)
                .HasForeignKey(u => u.LikerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // configure Messages Table
            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessageSent)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessageReceived)
                .OnDelete(DeleteBehavior.Restrict);
        }
        
    }
}