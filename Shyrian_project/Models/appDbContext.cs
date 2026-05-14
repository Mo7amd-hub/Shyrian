using Microsoft.EntityFrameworkCore;
using Shyrian_project.Models;
using System.Security.Cryptography;
using System.Text;

namespace Shyrian_project.Models
{
    public class appDbContext : DbContext
    {
        public appDbContext(DbContextOptions<appDbContext> options) : base(options) { }

        public DbSet<Governorate> Governorates { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<BloodType> BloodTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }
        public DbSet<DonationOffer> DonationOffers { get; set; }
        public DbSet<Admin> Admins { get; set; }

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


            modelBuilder.Entity<BloodRequest>()
                .HasOne(br => br.SelectedDonor)
                .WithMany()
                .HasForeignKey(br => br.SelectedDonorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DonationOffer>()
                .HasOne(d => d.BloodRequest)
                .WithMany(br => br.DonationOffers)
                .HasForeignKey(d => d.BloodRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DonationOffer>()
                .HasOne(d => d.Donor)
                .WithMany(u => u.DonationOffers)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.Restrict);


            // Data Seeding
         
            modelBuilder.Entity<Governorate>().HasData(
                new Governorate { Id = 1, Name = "أسيوط" },
                new Governorate { Id = 2, Name = "القاهرة" },
                new Governorate { Id = 3, Name = "الجيزة" },
                new Governorate { Id = 4, Name = "الإسكندرية" },
                new Governorate { Id = 5, Name = "المنصورة" },
                new Governorate { Id = 6, Name = "المنيا" },
                new Governorate { Id = 7, Name = "سوهاج" },
                new Governorate { Id = 8, Name = "قنا" },
                new Governorate { Id = 9, Name = "الأقصر" },
                new Governorate { Id = 10, Name = "أسوان" }
            );

            
            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Name = "أسيوط", GovernorateId = 1 },
                new City { Id = 2, Name = "أسيوط الجديدة", GovernorateId = 1 },
                new City { Id = 3, Name = "الفتح", GovernorateId = 1 },

                new City { Id = 4, Name = "مدينة نصر", GovernorateId = 2 },
                new City { Id = 5, Name = "مصر الجديدة", GovernorateId = 2 },
                new City { Id = 6, Name = "المعادي", GovernorateId = 2 },

                new City { Id = 7, Name = "6 أكتوبر", GovernorateId = 3 },
                new City { Id = 8, Name = "الدقي", GovernorateId = 3 },
                new City { Id = 9, Name = "الهرم", GovernorateId = 3 },

                new City { Id = 10, Name = "المنتزة", GovernorateId = 4 },
                new City { Id = 11, Name = "سموحة", GovernorateId = 4 },
                new City { Id = 12, Name = "محرم بك", GovernorateId = 4 },

                new City { Id = 13, Name = "المنصورة", GovernorateId = 5 },
                new City { Id = 14, Name = "طلخا", GovernorateId = 5 },
                new City { Id = 15, Name = "ميت غمر", GovernorateId = 5 },

                new City { Id = 16, Name = "المنيا", GovernorateId = 6 },
                new City { Id = 17, Name = "ملوي", GovernorateId = 6 },
                new City { Id = 18, Name = "مغاغة", GovernorateId = 6 },

                new City { Id = 19, Name = "سوهاج", GovernorateId = 7 },
                new City { Id = 20, Name = "طهطا", GovernorateId = 7 },
                new City { Id = 21, Name = "جرجا", GovernorateId = 7 },

                new City { Id = 22, Name = "قنا", GovernorateId = 8 },
                new City { Id = 23, Name = "نجع حمادي", GovernorateId = 8 },
                new City { Id = 24, Name = "قوص", GovernorateId = 8 },

                new City { Id = 25, Name = "الأقصر", GovernorateId = 9 },
                new City { Id = 26, Name = "إسنا", GovernorateId = 9 },
                new City { Id = 27, Name = "أرمنت", GovernorateId = 9 },

                new City { Id = 28, Name = "أسوان", GovernorateId = 10 },
                new City { Id = 29, Name = "كوم أمبو", GovernorateId = 10 },
                new City { Id = 30, Name = "إدفو", GovernorateId = 10 }
            );

            // seed data for blood types (dropdown list)
            modelBuilder.Entity<BloodType>().HasData(
                new BloodType { Id = 1, Name = "A+" },
                new BloodType { Id = 2, Name = "A-" },
                new BloodType { Id = 3, Name = "B+" },
                new BloodType { Id = 4, Name = "B-" },
                new BloodType { Id = 5, Name = "AB+" },
                new BloodType { Id = 6, Name = "AB-" },
                new BloodType { Id = 7, Name = "O+" },
                new BloodType { Id = 8, Name = "O-" }
            );

            // seed data for admin user
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    Email = "admin@shyrian.com",
                    Password = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("123456")))
                }
            );  
        }
    }
}