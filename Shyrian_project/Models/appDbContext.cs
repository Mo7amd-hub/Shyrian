using Microsoft.EntityFrameworkCore;
using Shyrian_project.Models;

namespace firstSection.Models
{
    public class appDbContext : DbContext
    {
        //DbContextOptions
        public appDbContext(
            DbContextOptions<appDbContext> options) : base(options) {
        }

        public DbSet<Governorate> Governorates { get; set; }
        public DbSet<City> Cities { get; set; } 
        public DbSet<BloodType> BloodTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<City>()
                .HasOne(c => c.Governorate)
                .WithMany(g => g.Cities)
                .HasForeignKey(c => c.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<User>()
                .HasOne(u => u.City)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CityId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<User>()
                .HasOne(u => u.BloodType)
                .WithMany(bt => bt.Users)
                .HasForeignKey(u => u.BloodTypeId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<BloodRequest>()
                .HasOne(br => br.Requester)
                .WithMany(u => u.MyRequests)
                .HasForeignKey(br => br.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<BloodRequest>()
                .HasOne(br => br.BloodType)
                .WithMany(bt => bt.BloodRequests)
                .HasForeignKey(br => br.BloodTypeId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<BloodRequest>()
                .HasOne(br => br.HospitalGovernorate)
                .WithMany()
                .HasForeignKey(br => br.HospitalGovernorateId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<BloodRequest>()
                .HasOne(br => br.HospitalCity)
                .WithMany()
                .HasForeignKey(br => br.HospitalCityId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(
        //        "Server=MG;Database=ShyrianDB;Trusted_Connection=True;TrustServerCertificate=True"
        //        );
        //}


    }
}
