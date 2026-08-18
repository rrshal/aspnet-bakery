namespace BakeryMongoApp.Models;

public class SearchOrderViewModel
{
    public Customer? Customer { get; set; }
    public List<CustomerOrder> Orders { get; set; } = new();
    public List<CustomerOrderItem> OrderItems { get; set; } = new();
    public List<MenuItem> MenuItems { get; set; } = new();
    public string? Message { get; set; }
}
