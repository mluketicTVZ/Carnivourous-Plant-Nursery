using Microsoft.AspNetCore.Mvc;
using Carnivorous_Plant_Nursery.Repositories;
using Carnivorous_Plant_Nursery.Models;

namespace Carnivorous_Plant_Nursery.Controllers
{
    [Route("inventory")]
    public class InventoryController : Controller
    {
        private readonly InventoryRepository _inventoryRepository;

        public InventoryController(InventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        [Route("")]
        public IActionResult Index([FromQuery] string searchTerm, [FromQuery] bool? webshopOnly)
        {
            var inventory = _inventoryRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLowerInvariant();
                inventory = inventory.Where(i => 
                    (i.ListingTitle?.ToLowerInvariant().Contains(term) ?? false) ||
                    (i.Taxonomy?.FullName?.ToLowerInvariant().Contains(term) ?? false) ||
                    (i.Taxonomy?.CommonName?.ToLowerInvariant().Contains(term) ?? false) ||
                    (i.SKU?.ToLowerInvariant().Contains(term) ?? false)
                ).ToList();
            }

            if (webshopOnly == true)
            {
                inventory = inventory.Where(i => i.IsAvailableInWebshop).ToList();
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.WebshopOnly = webshopOnly;

            return View(inventory);
        }

        [Route("{id:int}")]
        public IActionResult Details(int id)
        {
            var item = _inventoryRepository.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpGet]
        [Route("suggestions")]
        public IActionResult Suggestions([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Trim().Length < 1)
                return Json(new List<object>());

            var lowerTerm = term.Trim().ToLower();

            var all = _inventoryRepository.GetAll()
                .Where(i =>
                    (i.ListingTitle?.ToLower().Contains(lowerTerm) ?? false) ||
                    (i.Taxonomy?.FullName?.ToLower().Contains(lowerTerm) ?? false) ||
                    (i.Taxonomy?.CommonName?.ToLower().Contains(lowerTerm) ?? false) ||
                    (i.SKU?.ToLower().Contains(lowerTerm) ?? false))
                .ToList();

            var suggestions = new List<object>();

            // Taxonomy-based suggestions (grouped by CommonName)
            var taxonomySuggestions = all
                .Where(i => i.Taxonomy != null && !string.IsNullOrEmpty(i.Taxonomy.CommonName))
                .Select(i => new
                {
                    text = $"{i.Taxonomy!.CommonName} ({i.Taxonomy.FullName})",
                    value = i.Taxonomy.CommonName!
                })
                .DistinctBy(x => x.value)
                .OrderBy(x =>
                {
                    var idx = x.text.ToLower().IndexOf(lowerTerm);
                    return idx < 0 ? int.MaxValue : idx;
                });

            suggestions.AddRange(taxonomySuggestions);

            // SKU-based suggestions (only when the SKU itself matches the term)
            var skuSuggestions = all
                .Where(i => !string.IsNullOrEmpty(i.SKU) && i.SKU.ToLower().Contains(lowerTerm))
                .Select(i => new
                {
                    text = string.IsNullOrEmpty(i.Taxonomy?.CommonName)
                        ? $"SKU: {i.SKU}"
                        : $"SKU: {i.SKU} — {i.Taxonomy!.CommonName}",
                    value = i.SKU!
                })
                .DistinctBy(x => x.value)
                .OrderBy(x =>
                {
                    var idx = x.value.ToLower().IndexOf(lowerTerm);
                    return idx < 0 ? int.MaxValue : idx;
                });

            suggestions.AddRange(skuSuggestions);

            return Json(suggestions.Take(8).ToList());
        }
    }
}
