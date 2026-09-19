using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;

namespace OrbitAOS.Infrastructure.Data
{
    /// <summary>
    /// Application database context combining ASP.NET Core Identity with domain entities.
    /// Replaces the legacy ApplicationDbContext from net6.0 and upgrades to EF Core 8.
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

            builder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IdentityUserId).IsRequired().HasMaxLength(450);
                entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(256);
                entity.Property(e => e.Department).HasMaxLength(256);
                entity.Property(e => e.JobTitle).HasMaxLength(256);
                entity.HasIndex(e => e.IdentityUserId).IsUnique();
            });
        }
    }
}
