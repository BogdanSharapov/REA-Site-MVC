using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using REASite.Areas.Identity.Data;
using REASite.Models;
using System.Configuration;

namespace REASite.Data
{
    public class REASiteDbContext : IdentityDbContext<SiteUser>
    {
        public DbSet<Apartment> Apartments { get; set; } = null!;
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<ApartmentImage> ApartmentImages { get; set; } = null!;

        public REASiteDbContext(DbContextOptions<REASiteDbContext> options) : base(options)
        {

        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); //for identity

            modelBuilder.Entity<Apartment>()
                 .HasOne(a => a.Address)
                 .WithMany(a => a.Apartments)
                 .HasForeignKey(a => a.AddressId);

            modelBuilder.Entity<ApartmentComfort>()
                .HasKey(ac => new { ac.ApartmentId, ac.Comfort });

            modelBuilder.Entity<ApartmentComfort>()
                .HasOne(ac => ac.Apartment)
                .WithMany(a => a.ApartmentComforts)
                .HasForeignKey(ac => ac.ApartmentId);

            modelBuilder.Entity<ApartmentImage>()
                .HasOne(ai => ai.Apartment)
                .WithMany(a => a.Images)
                .HasForeignKey(ai => ai.ApartmentId);

            // Добавляем уникальный индекс на поля адреса
            modelBuilder.Entity<Address>()
                .HasIndex(a => new { a.Country, a.City, a.Street, a.HouseNum })
                .IsUnique();

            //modelBuilder.Entity<SiteUser>()
            //.Property(u => u.Gender)
            //.HasConversion<string>()
            //.HasColumnType("varchar(20)");

        }
    }

}
