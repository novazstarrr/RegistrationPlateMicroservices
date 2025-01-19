using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Catalog.API.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=sqlserver;Database=CatalogDb;User Id=sa;Password=YourStrongPassword123;TrustServerCertificate=true",
                options => options.EnableRetryOnFailure()
            );

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}