using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sanayii.Core.Entities;
using Sanayii.Entities;

namespace Snai3y.Repository.Data
{
    public class SanayiiContext : IdentityDbContext<AppUser>
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

            // Configure Table-per-Type (TPT) Inheritance
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Artisan>().ToTable("Artisans");
            modelBuilder.Entity<Admin>().ToTable("Admins");

            modelBuilder.Entity<Payment>()
            .Property(p => p.Method)
            .HasConversion<string>();

            // Configure foreign key constraints for Reviews table
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

            // Configure Violations Table
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

            // Audit Log Configuration
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
            // Configure the many-to-many relationship with the extra field in the join table
            modelBuilder.Entity<CustomerDiscount>()
                .HasKey(cd => new { cd.CustomerId, cd.DiscountId });

            modelBuilder.Entity<CustomerDiscount>()
                .HasOne(cd => cd.Customer)
                .WithMany(c => c.CustomerDiscounts)
                .HasForeignKey(cd => cd.CustomerId);

            modelBuilder.Entity<CustomerDiscount>()
                .HasOne(cd => cd.Discount)
                .WithMany(d => d.CustomerDiscounts)
                .HasForeignKey(cd => cd.DiscountId);

            // Optionally: You can specify the table name if you want to use a custom table name
            modelBuilder.Entity<CustomerDiscount>()
                .ToTable("CustomerDiscounts");
        }

        public DbSet<Artisan> Artisans { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}