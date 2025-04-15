using Microsoft.EntityFrameworkCore;
using REASite.Models;

namespace REASite.Data
{
    public class REASiteContext : DbContext
    {
        public DbSet<ApartmentModel> Apartments { get; set; }

        public REASiteContext(DbContextOptions<REASiteContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=REASitedb;Username=postgres;Password=4568");
        }
    }

}
