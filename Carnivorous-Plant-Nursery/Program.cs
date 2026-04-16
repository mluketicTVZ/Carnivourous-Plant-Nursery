using Carnivorous_Plant_Nursery.Models;
using Carnivorous_Plant_Nursery.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// ── Mock Repositories (replace with real implementations later) ──────────────
builder.Services.AddSingleton<TaxonomyMockRepository>();
builder.Services.AddSingleton<PlantMockRepository>();
builder.Services.AddSingleton<SeedBatchMockRepository>();
builder.Services.AddSingleton<InventoryMockRepository>();
builder.Services.AddSingleton<CareProfileMockRepository>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
