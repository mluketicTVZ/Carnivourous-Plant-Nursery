using Carnivorous_Plant_Nursery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
var app = builder.Build();

// ==========================================
// Lab 1: Object Creation and LINQ + Async
// ==========================================

// Simulation of fetching plant and seed data from the "database"
Console.WriteLine("=================================");
Console.WriteLine("Pokretanje simulacije dohvata inventara...");
var taxonomyList = await FetchInventoryAsync();

var webshopItems = GetWebshopItems(taxonomyList);
Console.WriteLine($"\nPronadeno {webshopItems.Count} predmeta dostupnih u webshopu:");
foreach(var wp in webshopItems)
{
    Console.WriteLine($"- {wp.ListingTitle} (Cijena: {wp.Price} EUR)");
}

var seedsForStratification = GetSeedsRequiringStratification(taxonomyList);
Console.WriteLine($"\nPronadeno {seedsForStratification.Count} serija sjemenki koje trebaju hladnu stratifikaciju.");

var lineagePlants = GetItemsWithKnownLineage(taxonomyList);
Console.WriteLine("\nBiljke s poznatim podrijetlom (Lineage):");
foreach (var lb in lineagePlants)
{
    Console.WriteLine($"- {lb.ListingTitle} ({lb.Taxonomy?.FullName ?? DisplayConstant.UnknownTaxonomy}): Generacija - {lb.Lineage?.Generation ?? DisplayConstant.UnknownGeneration}");
}
Console.WriteLine("=================================");

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

async Task<List<Taxonomy>> FetchInventoryAsync()
{
    await Task.Delay(1000);
    
    // 1. Dionaea muscipula
    var dionaeaTaxonomy = new Taxonomy
    {
        Id = 1, Genus = "Dionaea", Species = "muscipula",
        Cultivar = "B52", CommonName = "Venerina muholovka", CareProfileId = 1
    };
    
    // 2. Sarracenia
    var sarraceniaTaxonomy = new Taxonomy
    {
        Id = 2, Genus = "Sarracenia", Species = "purpurea",
        Cultivar = "", CommonName = "Ljubicasta saracenija", CareProfileId = 2
    };

    // 3. Drosera
    var droseraTaxonomy = new Taxonomy
    {
        Id = 3, Genus = "Drosera", Species = "capensis",
        Cultivar = "Alba", CommonName = "Kapska rosika", CareProfileId = 3
    };

    var lineage1 = new Lineage { Id = 101, MotherId = 1, FatherId = 2, Generation = "F1", IsClone = false, GeneticsDescription = "Dionaea B52 x Typical" };
    
    var plant1 = new Plant
    {
        Id = 1, TaxonomyId = 1, Taxonomy = dionaeaTaxonomy, 
        ListingTitle = "Velika Venerina muholovka (B52)", SKU = "PL-DIO-001",
        Price = 15.99m, IsAvailableInWebshop = true, Description = "Odrasla biljka klona B52.",
        DateAcquired = new DateTime(2023, 4, 15), InternalNotes = "Oprez pri pakiranju", LocationInNursery = "Staklenik sjev A",
        CurrentStage = PlantStage.Mature, HealthStatus = HealthState.Excellent,
        PotDiameterCm = 9.0m, PotHeightCm = 9.0m, EstimatedAgeAtAcquiryYears = 1,
        LineageId = 101, Lineage = lineage1
    };

    var plant2 = new Plant
    {
        Id = 2, TaxonomyId = 2, Taxonomy = sarraceniaTaxonomy,
        ListingTitle = "Saracenija purpurea - Odrasla", SKU = "PL-SAR-001",
        Price = 22.50m, IsAvailableInWebshop = true, Description = "Jako obojena sorta purpuree.",
        DateAcquired = new DateTime(2022, 5, 10), LocationInNursery = "Staklenik jug B",
        CurrentStage = PlantStage.Flowering, HealthStatus = HealthState.Good,
        PotDiameterCm = 12.0m, PotHeightCm = 12.0m, EstimatedAgeAtAcquiryYears = 2
    };

    var plant3 = new Plant
    {
        Id = 3, TaxonomyId = 3, Taxonomy = droseraTaxonomy,
        ListingTitle = "Drosera capensis 'Alba' Seedling", SKU = "PL-DRO-002",
        Price = 5.00m, IsAvailableInWebshop = false, Description = "Male sadnice drosere, jos rastu.",
        DateAcquired = DateTime.Now, LocationInNursery = "Polica pod lampama",
        CurrentStage = PlantStage.Seedling, HealthStatus = HealthState.Excellent,
        PotDiameterCm = 5.0m, PotHeightCm = 5.0m, EstimatedAgeAtAcquiryYears = 0
    };

    var seeds1 = new SeedBatch
    {
        Id = 4, TaxonomyId = 2, Taxonomy = sarraceniaTaxonomy,
        ListingTitle = "Sjemenke S. purpurea (20 kom)", SKU = "SD-SAR-001",
        Price = 3.99m, IsAvailableInWebshop = true, Description = "Svjeze sjemenke prikupljene prosle jeseni.",
        DateAcquired = new DateTime(2023, 10, 20), LocationInNursery = "Hladnjak C",
        SeedCount = 20, HarvestDate = new DateTime(2023, 10, 15),
        RequiresStratification = true, EstimatedGerminationRate = 0.85m
    };

    var seeds2 = new SeedBatch
    {
        Id = 5, TaxonomyId = 3, Taxonomy = droseraTaxonomy,
        ListingTitle = "Sjemenke D. capensis (100+)", SKU = "SD-DRO-001",
        Price = 2.50m, IsAvailableInWebshop = true, Description = "Mikroskopski sitno sjeme rosike, jako klijavo.",
        DateAcquired = new DateTime(2024, 1, 5), LocationInNursery = "Soba za klijanje",
        SeedCount = 150, HarvestDate = new DateTime(2023, 12, 10),
        RequiresStratification = false, EstimatedGerminationRate = 0.95m
    };

    dionaeaTaxonomy.InventoryItems.Add(plant1);
    sarraceniaTaxonomy.InventoryItems.Add(plant2);
    sarraceniaTaxonomy.InventoryItems.Add(seeds1);
    droseraTaxonomy.InventoryItems.Add(plant3);
    droseraTaxonomy.InventoryItems.Add(seeds2);

    return new List<Taxonomy> { dionaeaTaxonomy, sarraceniaTaxonomy, droseraTaxonomy };
}

List<InventoryItem> GetWebshopItems(List<Taxonomy> taxonomies)
{
    return taxonomies
        .SelectMany(t => t.InventoryItems)
        .Where(item => item.IsAvailableInWebshop)
        .OrderBy(item => item.Price)
        .ToList();
}

List<SeedBatch> GetSeedsRequiringStratification(List<Taxonomy> taxonomies)
{
    return taxonomies
        .SelectMany(t => t.InventoryItems)
        .OfType<SeedBatch>()
        .Where(s => s.RequiresStratification == true)
        .OrderByDescending(s => s.SeedCount)
        .ToList();
}

List<InventoryItem> GetItemsWithKnownLineage(List<Taxonomy> taxonomies)
{
    return taxonomies
        .SelectMany(t => t.InventoryItems)
        .Where(i => i.Lineage != null && (i.Lineage.MotherId != null || i.Lineage.FatherId != null))
        .ToList();
}
