using Microsoft.EntityFrameworkCore;
using MinimalAPI_DotNet6.Model;

namespace MinimalAPI_DotNet6.Data
{
    public class MinimalContextDb : DbContext
    {
        public MinimalContextDb(DbContextOptions<MinimalContextDb> options) : base(options) { }

        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Supplier>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Supplier>()
                .Property(e => e.Name)
                .IsRequired()
                .HasColumnType("varchar(200)");

            modelBuilder.Entity<Supplier>()
               .Property(e => e.DocumentationNumber)
               .IsRequired()
               .HasColumnType("varchar(14)");

            modelBuilder.Entity<Supplier>()
               .ToTable("Suppliers");

            base.OnModelCreating(modelBuilder);
        }
    }
}
