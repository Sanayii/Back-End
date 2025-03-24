using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Snai3y.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snai3y.Repository.Data
{
    class SanayiiContext : IdentityDbContext<AppUser>
    {
        public SanayiiContext(DbContextOptions<SanayiiContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite PK for UserPhones
            modelBuilder.Entity<UserPhones>().HasKey(UP => new
            {
                UP.UserId,
                UP.PhoneNumber
            });

            // Composite PK for PaymentMethods
            modelBuilder.Entity<PaymentMethods>().HasKey(PM => new
            {
                PM.PaymentId,
                PM.Method
            });

            // Composite PK for ServiceRequestPayment
            modelBuilder.Entity<ServiceRequestPayment>().HasKey(SRP => new
            {
                SRP.CustomerId,
                SRP.PaymentId,
                SRP.ServiceId
            });

            // Configure Identity Table Names (optional)
            modelBuilder.Entity<AppUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<string>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            // ✅ Use Table-Per-Type (TPT) to create separate tables
            modelBuilder.Entity<AppUser>().UseTptMappingStrategy();

            // ✅ Ensure Artisan and Admin `Id` is also an FK to `AppUser`
            modelBuilder.Entity<Artisan>()
                .ToTable("Artisans")
                .HasOne<AppUser>()
                .WithOne()
                .HasForeignKey<Artisan>(a => a.Id);

            modelBuilder.Entity<Admin>()
                .ToTable("Admins")
                .HasOne<AppUser>()
                .WithOne()
                .HasForeignKey<Admin>(a => a.Id);

            modelBuilder.Entity<Customer>()
                .ToTable("Customer")
                .HasOne<AppUser>()
                .WithOne()
                .HasForeignKey<Customer>(c => c.Id);
        }
        public DbSet<Artisan> Artisans { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
