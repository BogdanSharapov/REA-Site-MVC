using Microsoft.EntityFrameworkCore;
using REASite.Models;
using System.Configuration;

namespace REASite.Data
{
    public class REASiteDbContext : DbContext
    {
        public DbSet<ApartmentModel> Apartments { get; set; } = null!;
        public DbSet<Addres> Addres { get; set; } = null!;

        public REASiteDbContext(DbContextOptions<REASiteDbContext> options) : base(options)
        {
       
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=REASitedb;Username=postgres;Password=4568");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseIdentityColumns();
            /*modelBuilder.Entity<ApartmentModel>().HasData(
                new ApartmentModel { Id=1, Title = "Apartment1"},
                new ApartmentModel { Id = 2, Title = "Apartment2" },
                new ApartmentModel { Id = 3, Title = "Apartment3" },
                new ApartmentModel { Id = 4, Title = "Apartment4" }
                );*/
            
            
        }
    }

}
