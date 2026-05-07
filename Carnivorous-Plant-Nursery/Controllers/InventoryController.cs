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
    }
}
