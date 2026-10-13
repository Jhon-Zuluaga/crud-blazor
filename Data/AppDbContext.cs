

using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> products => Set<Product>();
    public DbSet<InventoryMovement> Movements => Set<InventoryMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.HasIndex(p => p.Code).IsUnique();
            e.Property(p => p.PurchasePrice).HasPrecision(18, 2);
            e.Property(p => p.SellingPrice).HasPrecision(18, 2);

            e.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            
            e.HasOne(p => p.Supplier)
                .WithMany(pr => pr.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InventoryMovement>(e =>
        {
            e.Property(m => m.type).HasConversion<string>().HasMaxLength(20);

            e.HasOne(m => m.Product)
                .WithMany(p => p.Movements)
                .HasForeignKey(m => m.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}