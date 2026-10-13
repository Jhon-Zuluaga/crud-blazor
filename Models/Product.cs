

using System.ComponentModel.DataAnnotations;

public class Product
{
    public int Id { get; set;}

    [Required(ErrorMessage = "Code is required")]
    [StringLength(30)]
    public string Code { get; set;} = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(120)]
    public string Name { get; set;} = string.Empty;

    [StringLength(300)]
    public string? Description { get; set;}

    [Range(0, 1_000_000, ErrorMessage = "Invalid purchase price")]
    public decimal PurchasePrice { get; set;}

    [Range(0.01, 1_000_000, ErrorMessage = "The sale price must be greater than 0")]
    public decimal SellingPrice { get; set;}

    [Range(0, int.MaxValue)]
    public int CurrentStock { get; set;}

    [Range(0, int.MaxValue)]
    public int MinimumStock { get; set;}

    public bool Active { get; set;} = true;

    [Range(1, int.MaxValue, ErrorMessage = "Select a category")]
    public int CategoryId { get; set;}
    public Category? Category { get; set;}

    [Range(1, int.MaxValue, ErrorMessage = "Select a supplier")]
    public int SupplierId { get; set;}
    public Supplier? Supplier { get; set;}

    public List<InventoryMovement> Movements { get; set;} = [];
}