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
        
    }
}