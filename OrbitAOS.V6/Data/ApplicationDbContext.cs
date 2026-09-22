using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Entities;

namespace OrbitAOS.V6.Data
{
    /// <summary>
    /// Application database context – EF Core 8.
    /// Replaces the legacy EF6 DbContext and ApplicationDbContext from ASP.NET Identity 2.x.
    /// Inherits from IdentityDbContext to include ASP.NET Core Identity tables.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Domain entity DbSets – add your entities here
        public DbSet<OrbitItem> OrbitItems => Set<OrbitItem>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure OrbitItem entity
            builder.Entity<OrbitItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
