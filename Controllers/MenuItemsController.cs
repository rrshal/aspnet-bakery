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

    public async Task<IActionResult> Edit(string id)
    {
        var menuItem = await _menuItemsCollection.Find(m => m.Id == id).FirstOrDefaultAsync();

        if (menuItem == null)
        {
            return RedirectToAction(nameof(Index));
        }

        return View(menuItem);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, MenuItem menuItem)
    {
        await _menuItemsCollection.ReplaceOneAsync(m => m.Id == id, menuItem);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(string id)
    {
        var menuItem = await _menuItemsCollection.Find(m => m.Id == id).FirstOrDefaultAsync();

        if (menuItem == null)
        {
            return RedirectToAction(nameof(Index));
        }

        return View(menuItem);
    }

    public async Task<IActionResult> Delete(string id)
    {
        var menuItem = await _menuItemsCollection.Find(m => m.Id == id).FirstOrDefaultAsync();

        if (menuItem == null)
        {
            return RedirectToAction(nameof(Index));
        }

        return View(menuItem);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await _menuItemsCollection.DeleteOneAsync(m => m.Id == id);
        return RedirectToAction(nameof(Index));
    }
}
