using BakeryMongoApp.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<BakeryDatabaseSettings>(
    builder.Configuration.GetSection("BakeryDatabase"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<BakeryDatabaseSettings>>().Value;
    return new MongoClient(cfg.ConnectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<BakeryDatabaseSettings>>().Value;
    return sp.GetRequiredService<IMongoClient>().GetDatabase(cfg.DatabaseName);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

    database.GetCollection<CustomerOrder>("CustomerOrder")
    .Indexes.CreateOne(new CreateIndexModel<CustomerOrder>("{ CustomerId: 1 }"));
    database.GetCollection<CustomerOrderItem>("CustomerOrderItem")
        .Indexes.CreateOne(new CreateIndexModel<CustomerOrderItem>("{ CustomerOrderId: 1 }"));
    database.GetCollection<CustomerOrderItem>("CustomerOrderItem")
        .Indexes.CreateOne(new CreateIndexModel<CustomerOrderItem>("{ MenuItemId: 1 }"));

    var collection = database.GetCollection<MenuItem>("MenuItem");
    if (!collection.Find(m => true).Any())
    {
        var defaultMenu = new List<MenuItem>
        {
            new MenuItem {
                Name = "Cookie", Price = 14, ItemType = 1, HighSugar = true },
            new MenuItem {
                Name = "Croissant", Price = 21, ItemType = 1, HighSugar = false },
            new MenuItem {
                Name = "Tart", Price = 18, ItemType = 1, HighSugar = false },
            new MenuItem {
                Name = "Brownie", Price = 16, ItemType = 1, HighSugar = true },
            new MenuItem {
                Name = "IceTea", Price = 17, ItemType = 2, Size = "L" },
            new MenuItem {
                Name = "Matcha", Price = 23, ItemType = 2, Size = "M" },
            new MenuItem {
                Name = "Milkshake", Price = 19, ItemType = 2, Size = "S" },
            new MenuItem {
                Name = "Chocolate Cake", Price = 33, ItemType = 3, HighSugar = true, Layers = 2 },
            new MenuItem {
                Name = "Vanilla Cake", Price = 35, ItemType = 3, HighSugar = false, Layers = 4 },
            new MenuItem {
                Name = "Espresso", Price = 20, ItemType = 4, Size = "S", IsHot = true },
            new MenuItem {
                Name = "Americano", Price = 12, ItemType = 4, Size = "L", IsHot = false }
        };
        collection.InsertMany(defaultMenu);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
