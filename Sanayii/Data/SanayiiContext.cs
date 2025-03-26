using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sanayii.Core.Entities;
using Sanayii.Entities;
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

            //✅Configure Violations Table==
            modelBuilder.Entity<Violation>(entity =>
            {
                entity.HasKey(v => v.Id);

                entity.Property(v => v.Id)
                      .ValueGeneratedOnAdd();

                entity.HasOne(v => v.Contract)
                      .WithMany(c => c.Violations)
                      .HasForeignKey(v => v.ContractId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
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

            modelBuilder.Entity<ServiceRequestPayment>()
                .HasKey(srp => new { srp.CustomerId, srp.PaymentId, srp.ServiceId });

            //✅ Configure foreign key constraints for Reviews table
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Artisan)
                .WithMany(a => a.Reviews)
                .HasForeignKey(r => r.ArtisanId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Service)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);

            //✅Audit Log Configuration
            modelBuilder.Entity<AuditLog>(entity =>
            {
                // Set Primary Key
                entity.HasKey(a => a.AuditId);

                // Make TableName Required and Limit Length
                entity.Property(a => a.TableName)
                      .IsRequired()
                      .HasMaxLength(100);

                // Make Type Required and Limit Length
                entity.Property(a => a.Type)
                      .IsRequired()
                      .HasMaxLength(50);

                // Make UserId Optional but with a Max Length
                entity.Property(a => a.UserId)
                      .HasMaxLength(450);

                // Ensure PrimaryKey is Stored as JSON
                entity.Property(a => a.PrimaryKey)
                      .HasColumnType("NVARCHAR(MAX)");

                // Store OldValues and NewValues as JSON (optional)
                entity.Property(a => a.OldValues)
                      .HasColumnType("NVARCHAR(MAX)");

                entity.Property(a => a.NewValues)
                      .HasColumnType("NVARCHAR(MAX)");

                // Store AffectedColumns as JSON (optional)
                entity.Property(a => a.AffectedColumns)
                      .HasColumnType("NVARCHAR(MAX)");

                // Index for Faster Queries on TableName and Timestamp
                entity.HasIndex(a => a.TableName);
                entity.HasIndex(a => a.Timestamp);
            });
        }

        //✅DbSets Properties
        public DbSet<Artisan> Artisans { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<AdminNotification> AdminNotifications { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
        public DbSet<ServiceRequestPayment> ServiceRequestPayments { get; set; }
        public DbSet<UserPhones> UserPhones { get; set; }
        public DbSet<Violation> Payments { get; set; }
        public DbSet<Violation> Violations { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        
    }
}
