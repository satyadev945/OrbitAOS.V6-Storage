using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OrbitAOS.V6.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Set default schema to "public" for PostgreSQL
            builder.HasDefaultSchema("public");

            // Configure PostgreSQL extensions
            builder.HasPostgresExtension("uuid-ossp");

            // Configure migrations history table with snake_case naming
            // (also configured in UseNpgsql options in Program.cs)
        }
    }
}
