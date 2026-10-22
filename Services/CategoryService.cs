

using Microsoft.EntityFrameworkCore;

public class CategoryService(IDbContextFactory<AppDbContext> factory)
{
    public async Task<List<Category>> ListCategory(string ? search = null)
    {
        await using var db = await factory.CreateDbContextAsync();

        var query = db.Categories.AsNoTracking();

        if(!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{search}"));

        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<Category?> GetAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Categories.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task CreateAsync(Category category)
    {
        await using var db = await factory.CreateDbContextAsync();
        db.Categories.Add(category);
        await db.SaveChangesAsync();
    }

     public async Task updateAsync(Category category)
    {
        await using var db = await factory.CreateDbContextAsync();
        db.Categories.Update(category);
        await db.SaveChangesAsync();
    }

    public async Task<string?> DeleteAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();

        if(await db.products.AnyAsync(p => p.CategoryId == id))
            return "Cannot be deleted: The category has associated products.";
        
        var category = await db.Categories.FindAsync(id);
        if(category is null) return "The category no longer exists.";

        db.Categories.Remove(category);
        await db.SaveChangesAsync();
        return null;
    }
}