using BakeryMongoApp.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BakeryMongoApp.Controllers;

public class MenuItemsController : Controller
{
    private readonly IMongoCollection<MenuItem> _menuItemsCollection;

    public MenuItemsController(IConfiguration configuration)
    {
        var connectionString = configuration["BakeryDatabase:ConnectionString"];
        var databaseName = configuration["BakeryDatabase:DatabaseName"];

        var mongoClient = new MongoClient(connectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseName);

        _menuItemsCollection = mongoDatabase.GetCollection<MenuItem>("MenuItem");
    }

    public async Task<IActionResult> Index()
    {
        var existingItems = await _menuItemsCollection.Find(m => true).ToListAsync();

        if (existingItems.Count == 0)
        {
            var defaultMenu = new List<MenuItem>
            {
                new MenuItem { 
                    Name = "Cookie", Price = 14.0, ItemType = 1, HighSugar = true },
                new MenuItem {
                    Name = "Croissant", Price = 21.0, ItemType = 1, HighSugar = false },
                new MenuItem { 
                    Name = "Tart", Price = 18.0, ItemType = 1, HighSugar = false },
                new MenuItem { 
                    Name = "Brownie", Price = 16.0, ItemType = 1, HighSugar = true },
                new MenuItem { 
                    Name = "IceTea", Price = 17.0, ItemType = 2, Size = "L" },
                new MenuItem { 
                    Name = "Matcha", Price = 23.0, ItemType = 2, Size = "M" },
                new MenuItem {
                    Name = "Milkshake", Price = 19.0, ItemType = 2, Size = "S" },
                new MenuItem { 
                    Name = "Chocolate Cake", Price = 33.0, ItemType = 3, HighSugar = true, Layers = 2 },
                new MenuItem { 
                    Name = "Vanilla Cake", Price = 35.0, ItemType = 3, HighSugar = false, Layers = 4 },
                new MenuItem { 
                    Name = "Espresso", Price = 20.0, ItemType = 4, Size = "S", IsHot = true },
                new MenuItem { 
                    Name = "Americano", Price = 12.0, ItemType = 4, Size = "L", IsHot = false }
            };
            await _menuItemsCollection.InsertManyAsync(defaultMenu);
            existingItems = await _menuItemsCollection.Find(m => true).ToListAsync();
        }

        return View(existingItems);
    }

    public async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(MenuItem menuItem)
    {
        await _menuItemsCollection.InsertOneAsync(menuItem);
        return RedirectToAction("Index");
    }
}
