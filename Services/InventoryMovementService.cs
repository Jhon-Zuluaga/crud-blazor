
using Microsoft.EntityFrameworkCore;

namespace crud_blazor.Services;

public class InventoryMovementService(IDbContextFactory<AppDbContext> factory)
{
    public async Task<List<InventoryMovement>> ListAsync(int? productId = null)
    {
        await using var db = await factory.CreateDbContextAsync();

        var query = db.Movements
            .AsNoTracking()
            .Include(m => m.Product)
            .AsQueryable();

        if (productId is not null)
            query = query.Where(m => m.ProductId == productId);

        return await query.OrderByDescending(m => m.Date).ToListAsync();
    }

    /// Returns null if registered, or the error message.
    public async Task<string?> RegisterAsync(InventoryMovement movement)
    {
        if (movement.type == TransactionType.Adjustment)
            return "Adjustments are not supported yet.";

        if (movement.Quantity <= 0)
            return "The quantity must be greater than 0.";

        await using var db = await factory.CreateDbContextAsync();
        await using var tx = await db.Database.BeginTransactionAsync();

        var product = await db.products
            .AsNoTracking()
            .Where(p => p.Id == movement.ProductId)
            .Select(p => new { p.Active })
            .FirstOrDefaultAsync();

        if (product is null) return "The product doesn't exist.";
        if (!product.Active) return "You can't record movements for an inactive product.";

        var delta = movement.type == TransactionType.Entry ? movement.Quantity : -movement.Quantity;

        // Atomic update: only applies if the stock doesn't go negative
        var rows = await db.products
            .Where(p => p.Id == movement.ProductId && p.CurrentStock + delta >= 0)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.CurrentStock, p => p.CurrentStock + delta));

        if (rows == 0) return "Insufficient stock to record the exit.";

        movement.Date = DateTime.UtcNow;
        movement.Product = null;
        db.Movements.Add(movement);
        await db.SaveChangesAsync();

        await tx.CommitAsync();
        return null;
    }
}