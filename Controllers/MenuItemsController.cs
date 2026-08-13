using BakeryMongoApp.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BakeryMongoApp.Controllers;

public class MenuItemsController : Controller
{
    private readonly IMongoCollection<MenuItem> _menuItemsCollection;

    public MenuItemsController(IMongoDatabase database)
    {
        _menuItemsCollection = database.GetCollection<MenuItem>("MenuItem");
    }

    public async Task<IActionResult> Index()
    {
        var existingItems = await _menuItemsCollection.Find(m => true).ToListAsync();
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
        return RedirectToAction(nameof(Index));
    }
}
