

using System.ComponentModel.DataAnnotations;

public class Category
{
    public int Id { get; set;}

    [Required(ErrorMessage = "Name is required")]
    [StringLength(80)]
    public string Name { get; set;} = string.Empty;

    [StringLength(250)]
    public string? Description { get; set;}

    public List<Product> Products { get; set;} = [];
}