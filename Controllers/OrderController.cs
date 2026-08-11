using BakeryMongoApp.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BakeryMongoApp.Controllers;

public class OrderController : Controller {
    private readonly IMongoCollection<Customer> _customerCollection;
    private readonly IMongoCollection<CustomerOrder> _customerOrderCollection;
    private readonly IMongoCollection<CustomerOrderItem> _customerOrderItemCollection;
    private readonly IMongoCollection<MenuItem> _menuItemCollection;

    public OrderController (IConfiguration configuration) {
        var connectionString = configuration["BakeryDatabase:ConnectionString"];
        var databaseName = configuration["BakeryDatabase:DatabaseName"];

        var mongoClient = new MongoClient(connectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseName);

        _customerCollection = mongoDatabase.GetCollection<Customer>("Customer");
        _customerOrderCollection = mongoDatabase.GetCollection<CustomerOrder>("CustomerOrder");
        _customerOrderItemCollection = mongoDatabase.GetCollection<CustomerOrderItem>("CustomerOrderItem");
        _menuItemCollection = mongoDatabase.GetCollection<MenuItem>("MenuItem");
    }

    public async Task<IActionResult> Create() {
        var menuItems = await _menuItemCollection.Find (m => true).ToListAsync();
        ViewBag.MenuItems = menuItems;
      return View();

    }

    [HttpPost]
    public async Task<IActionResult> Create (string name, string mobileNo, string[] menuItemIds, int[] quantities) {
        var customer = await _customerCollection.Find(c => c.MobileNo == mobileNo).FirstOrDefaultAsync();
        string customerId;
        
        if(customer == null) {
            var newCust = new Customer {
                Name = name, 
                MobileNo = mobileNo
            };

            await _customerCollection.InsertOneAsync(newCust);
            customerId = newCust.Id!;
        }
            else {
            customerId = customer.Id!;
            }

        double orderTotalPrice =0;
            for (int i = 0; i < menuItemIds.Length; i++)
            {
                int qty = quantities[i];
                if (qty > 0)
                {
                    string itemId = menuItemIds[i];
                var item = await _menuItemCollection.Find(m => m.Id == itemId).FirstOrDefaultAsync();
                if (item != null)
                {
                    orderTotalPrice += item.Price * qty;
                }
                }
            }

            var order = new CustomerOrder {
                CustomerId = customerId,
                TotalPrice = orderTotalPrice
            };
            
        await _customerOrderCollection.InsertOneAsync(order);

        for (int i = 0; i < menuItemIds.Length; i++) {
        int qty = quantities[i];
        if (qty > 0)
        {
            string itemId = menuItemIds[i];
                var item = await _menuItemCollection.Find(m => m.Id == itemId).FirstOrDefaultAsync();
                if (item != null)
                {
            var orderItem = new CustomerOrderItem() {
                CustomerOrderId = order.Id,
                MenuItemId = itemId,
                Quantity = qty,
                Price = item.Price
            };

                    await _customerOrderItemCollection.InsertOneAsync(orderItem);
                }
        }
    }

        ViewBag.MenuItems = await _menuItemCollection.Find(m => true).ToListAsync();
        ViewBag.Message = "Order was placed successfully";
    
    return View();
    }

    public async Task<IActionResult> Delete() {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Delete (string mobileNo) {
        var customer = await _customerCollection.Find(cust => cust.MobileNo == mobileNo).FirstOrDefaultAsync();

        if (customer == null) {
            ViewBag.Message = "Customer with that phone number was not found";
            return View();
        }

        var order = await _customerOrderCollection.Find(ord => ord.CustomerId == customer.Id).FirstOrDefaultAsync();

        if (order == null) {
            ViewBag.Message = "could not find orders for customer";
            return View();
        }

        await _customerOrderItemCollection.DeleteManyAsync(item => item.CustomerOrderId == order.Id);
        await _customerOrderCollection.DeleteOneAsync(o => o.Id == order.Id);

        ViewBag.Message = "Order was cancelled successfully";
        return View();
    }

    public async Task<IActionResult> SearchOrder (string mobileNo)
{
    var cust = await _customerCollection.Find(cust => cust.MobileNo == mobileNo).FirstOrDefaultAsync();
    
    if (cust == null) {
        ViewBag.Message = "could not find order";
            return View();
    }

    var order = await _customerOrderCollection.Find(ord => ord.CustomerId == cust.Id).FirstOrDefaultAsync();

    if (order == null) {
        ViewBag.Message = "could not find order";
            return View();
    }

    var orderItems = await _customerOrderItemCollection.Find(item => item.CustomerOrderId == order.Id).ToListAsync();
    
    ViewBag.Cust = cust;
    ViewBag.Order = order;
    ViewBag.OrderItems = orderItems;
    ViewBag.MenuItems = await _menuItemCollection.Find(m => true).ToListAsync();
    
    return View();
}
        
    
    
}