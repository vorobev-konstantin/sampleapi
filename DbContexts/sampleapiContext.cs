using Microsoft.EntityFrameworkCore;
using sampleapi.Entities;

namespace sampleapi.DbContexts
{
    public class sampleapiContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public sampleapiContext(DbContextOptions<sampleapiContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                 .HasData(
                new User("peskovv", "Vladislav", "Peskov", "peskovv@ya.ru")
                {
                    Id = 1
                },
                new User("fedorovr", "Roman", "Fedorov", "feforovr@ya.ru")
                {
                    Id = 2,
                    Phone = "+73450934543"
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
