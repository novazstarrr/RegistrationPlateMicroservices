using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Plate> Plates { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Plate>(builder =>
            {
                builder.ToTable("Plates");
                builder.HasKey(p => p.Id);

                builder.Property(p => p.PurchasePrice)
                    .HasConversion(
                        v => (int)(v * 100),
                        v => v / 100m)
                    .HasColumnType("INTEGER")
                    .IsRequired();

                builder.Property(p => p.SalePrice)
                    .HasConversion(
                        v => (int)(v * 100),
                        v => v / 100m)
                    .HasColumnType("INTEGER")
                    .IsRequired();
            });

            modelBuilder.Entity<AuditLog>(builder =>
            {
                builder.ToTable("AuditLogs");
                builder.HasKey(a => a.Id);

                builder.Property(a => a.Id)
                    .HasColumnType("uniqueidentifier");

                builder.Property(a => a.PlateId)
                    .HasColumnType("uniqueidentifier")
                    .IsRequired();

                builder.Property(a => a.Registration)
                    .HasColumnType("nvarchar(50)")
                    .IsRequired();

                builder.Property(a => a.Action)
                    .HasColumnType("nvarchar(50)")
                    .IsRequired();

                builder.Property(a => a.Timestamp)
                    .HasColumnType("datetime2")
                    .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
