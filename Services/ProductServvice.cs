
using Microsoft.EntityFrameworkCore;

public class ProductService(IDbContextFactory<AppDbContext> factory)
{
    public async Task<List<Product>> ListProduct(string ? search = null)
    {
        await using var db = await factory.CreateDbContextAsync();

        var query = db.products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .AsNoTracking();

        if(!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => EF.Functions.ILike(p.Code, $"%{search}%"));
        
        return await query.OrderBy(p => p.Code).ToListAsync();
    }

    public async Task<Product?> GetAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<string?> CreateAsync(Product product)
    {
        await using var db = await factory.CreateDbContextAsync();
        product.Code = product.Code.Trim().ToUpperInvariant();

        if(await db.products.AnyAsync(p => p.Code == product.Code))
            return $"A product with the code already exists.";
        
        db.products.Add(product);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return "Unable to save. The code may already exist, or the category/provider may be invalid.";

        }

        return null;
    }


    public async Task<string?> UpdateAsync(Product product)
    {
        await using var db = await factory.CreateDbContextAsync();

        product.Code = product.Code.Trim().ToUpperInvariant();

        // Excluir el propio producto
        if (await db.products.AnyAsync(p => p.Code == product.Code && p.Id != product.Id))
            return $"Another product with the code already exists.";
        
        db.products.Update(product);

        // El stock solo debe cambiar por movimientos, nunca desde form
        db.Entry(product).Property(p => p.CurrentStock).IsModified = false;

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return "It could not be saved. The code may already exist, or the category/provider may be invalid.";
        }
        return null;
    }


    public async Task<string?> DeleteAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();

        if(await db.Movements.AnyAsync(p => p.ProductId == id))
            return "Cannot be deleted: The product has inventory transactions. Mark it as inactive.";

        var producto = await db.products.FindAsync(id);
        if(producto is null) return "The product is no longer available.";

        db.products.Remove(producto);
        await db.SaveChangesAsync();
        return null;
    }
}