

using System.ComponentModel.DataAnnotations;

public class Supplier
{
    public int Id { get; set;}

    [Required(ErrorMessage = "Name is required")]
    [StringLength(120)]
    public string Name { get; set;} = string.Empty;

    [StringLength(100)]
    public string? Contact { get; set;}

    [Phone(ErrorMessage = "Invalid phone number")]
    [StringLength(30)]
    public string? Phone { get; set;}

    [EmailAddress(ErrorMessage = "Invalid email")]
    [StringLength(120)]
    public string? Email { get; set;}

    [StringLength(200)]
    public string? Address { get; set;}

    public bool Active { get; set;} = true;

    public List<Product> Products { get; set;} = [];
}