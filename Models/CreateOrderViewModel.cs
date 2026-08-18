namespace BakeryMongoApp.Models;

public class CreateOrderViewModel
{
    public List<MenuItem> MenuItems { get; set; } = new();
    public string? Message { get; set; }
}
