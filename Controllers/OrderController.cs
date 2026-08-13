using BakeryMongoApp.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BakeryMongoApp.Controllers;

public class OrderController : Controller
{
    private readonly IMongoCollection<Customer> _customerCollection;
    private readonly IMongoCollection<CustomerOrder> _customerOrderCollection;
    private readonly IMongoCollection<CustomerOrderItem> _customerOrderItemCollection;
    private readonly IMongoCollection<MenuItem> _menuItemCollection;

    public OrderController(IMongoDatabase database)
    {
        _customerCollection = database.GetCollection<Customer>("Customer");
        _customerOrderCollection = database.GetCollection<CustomerOrder>("CustomerOrder");
        _customerOrderItemCollection = database.GetCollection<CustomerOrderItem>("CustomerOrderItem");
        _menuItemCollection = database.GetCollection<MenuItem>("MenuItem");
    }

    public async Task<IActionResult> Create()
    {
        var menuItems = await _menuItemCollection.Find(m => true).ToListAsync();
        ViewBag.MenuItems = menuItems;
        return View();

    }

    [HttpPost]
    public async Task<IActionResult> Create(string name, string mobileNo, string[] menuItemIds, int[] quantities)
    {
        var customer = await _customerCollection.Find(c => c.MobileNo == mobileNo).FirstOrDefaultAsync();
        string customerId;

        if (customer == null)
        {
            var newCust = new Customer
            {
                Name = name,
                MobileNo = mobileNo
            };

            await _customerCollection.InsertOneAsync(newCust);
            customerId = newCust.Id!;
        }
        else
        {
            customerId = customer.Id!;
        }

        decimal orderTotalPrice = 0;
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

        var order = new CustomerOrder
        {
            CustomerId = customerId,
            TotalPrice = orderTotalPrice
        };

        await _customerOrderCollection.InsertOneAsync(order);

        for (int i = 0; i < menuItemIds.Length; i++)
        {
            int qty = quantities[i];
            if (qty > 0)
            {
                string itemId = menuItemIds[i];
                var item = await _menuItemCollection.Find(m => m.Id == itemId).FirstOrDefaultAsync();
                if (item != null)
                {
                    var orderItem = new CustomerOrderItem()
                    {
                        CustomerOrderId = order.Id,
                        MenuItemId = itemId,
                        Quantity = qty,
                        Price = item.Price
                    };

                    await _customerOrderItemCollection.InsertOneAsync(orderItem);
                }
            }
        }

        TempData["Message"] = "Order was placed successfully";
        return RedirectToAction(nameof(Create));
    }

    public async Task<IActionResult> Delete()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string orderId)
    {
        var order = await _customerOrderCollection.Find(o => o.Id == orderId).FirstOrDefaultAsync();

        if (order == null)
        {
            TempData["Message"] = "Order not found";
            return RedirectToAction(nameof(SearchOrder));
        }

        await _customerOrderItemCollection.DeleteManyAsync(item => item.CustomerOrderId == order.Id);
        await _customerOrderCollection.DeleteOneAsync(o => o.Id == order.Id);

        TempData["Message"] = "Order was cancelled successfully";
        return RedirectToAction(nameof(SearchOrder));
    }

    public async Task<IActionResult> SearchOrder(string mobileNo)
    {
        var cust = await _customerCollection.Find(cust => cust.MobileNo == mobileNo).FirstOrDefaultAsync();

        if (cust == null)
        {
            ViewBag.Message = "could not find order";
            return View();
        }

        var orders = await _customerOrderCollection.Find(ord => ord.CustomerId == cust.Id).ToListAsync();

        if (orders.Count == 0)
        {
            ViewBag.Message = "could not find order";
            return View();
        }

        var allOrderItems = new List<CustomerOrderItem>();
        foreach (var order in orders)
        {
            var items = await _customerOrderItemCollection.Find(item => item.CustomerOrderId == order.Id).ToListAsync();
            allOrderItems.AddRange(items);
        }

        ViewBag.Cust = cust;
        ViewBag.Orders = orders;
        ViewBag.OrderItems = allOrderItems;
        ViewBag.MenuItems = await _menuItemCollection.Find(m => true).ToListAsync();

        return View();
    }



}