
using System.ComponentModel.DataAnnotations;


public class InventoryMovement
{
    public int Id { get; set;}

    public int ProductId { get; set;}
    public Product? Product { get; set;}

    public TransactionType type { get; set;}

    [Range(1, int.MaxValue, ErrorMessage = "The amount must be greater than 0")]
    public int Quantity { get; set;}

    public DateTime Date { get; set;} = DateTime.UtcNow;

    [StringLength(250)]
    public string? Reason { get; set;}
}