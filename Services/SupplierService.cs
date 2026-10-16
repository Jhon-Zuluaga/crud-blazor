

using Microsoft.EntityFrameworkCore;

public class SupplierService(IDbContextFactory<AppDbContext> factory)
{
    public async Task<List<Supplier>> ListAsync(string? search = null)
    {
        await using var db = await factory.CreateDbContextAsync();

        var query = db.Suppliers.AsNoTracking();

        if(!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{search}%"));

        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<Supplier?> GetAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Suppliers.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task CreateAsync(Supplier supplier)
    {
        await using var db = await factory.CreateDbContextAsync();
        db.Suppliers.Add(supplier);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Supplier supplier)
    {
        await using var db = await factory.CreateDbContextAsync();
        db.Suppliers.Update(supplier);
        await db.SaveChangesAsync();
    }

    // Devuelve null si se eliminó, o el mensaje de error.
    public async Task<string?> DeleteAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();

        if(await db.products.AnyAsync(p => p.SupplierId == id))
            return "Cannot be deleted: The supplier has associated products. Mark it as inactive.";
        
        var supplier = await db.Suppliers.FindAsync(id);
        if(supplier is null) return "The supplier no longer exists.";

        db.Suppliers.Remove(supplier);
        await db.SaveChangesAsync();
        return null;
    }
}