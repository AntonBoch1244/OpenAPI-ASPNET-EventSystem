using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OpenAPIASPNET.Contexts
{
    public class OpenAPIASPNETContext : DbContext
    {
        public DbSet<Models.Events> Events { get; set; }
        public OpenAPIASPNETContext(DbContextOptions options) : base(options)
        {
        }
        public OpenAPIASPNETContext() {
            Database.Migrate();
        }

        public class OpenAPIASPNETContextFactory : IDesignTimeDbContextFactory<OpenAPIASPNETContext>
        {
            public OpenAPIASPNETContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<OpenAPIASPNETContext>();
                optionsBuilder.UseNpgsql();

                return new OpenAPIASPNETContext(optionsBuilder.Options);
            }
        }
    }
}
