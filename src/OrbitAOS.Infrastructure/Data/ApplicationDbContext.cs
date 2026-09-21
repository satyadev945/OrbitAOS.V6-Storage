using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;

namespace OrbitAOS.Infrastructure.Data
{
    /// <summary>
    /// Application database context for .NET 8.
    /// Extends IdentityDbContext to include ASP.NET Core Identity tables.
    /// Migrated from EF Core 6 to EF Core 8 with updated API usage.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure UserProfile entity
            builder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
                entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(256);
                entity.Property(e => e.Bio).HasMaxLength(1000);
                entity.Property(e => e.AvatarUrl).HasMaxLength(500);
                entity.HasIndex(e => e.UserId).IsUnique();
            });
        }
    }
}
