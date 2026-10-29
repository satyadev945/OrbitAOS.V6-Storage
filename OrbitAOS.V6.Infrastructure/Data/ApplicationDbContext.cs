using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Infrastructure.Services;

namespace OrbitAOS.V6.Infrastructure.Data
{
    /// <summary>
    /// Application database context with Identity support
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        private readonly ICurrentUserService? _currentUserService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService? currentUserService = null)
            : base(options)
        {
            _currentUserService = currentUserService;
        }

        // DbSets for domain entities
        public DbSet<SampleEntity> SampleEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure entity relationships and constraints
            builder.Entity<SampleEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.HasQueryFilter(e => !e.IsDeleted); // Global query filter for soft delete
            });

            // Add seed data if needed
            // builder.Entity<SampleEntity>().HasData(...);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Automatically set audit fields
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Domain.Common.BaseEntity && 
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (Domain.Common.BaseEntity)entry.Entity;
                var currentUser = _currentUserService?.UserName ?? "System";
                
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedDate = DateTime.UtcNow;
                    entity.CreatedBy = currentUser;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.ModifiedDate = DateTime.UtcNow;
                    entity.ModifiedBy = currentUser;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
