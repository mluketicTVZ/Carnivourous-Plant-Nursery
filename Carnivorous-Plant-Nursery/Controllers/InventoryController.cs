using Microsoft.AspNetCore.Mvc;
using Carnivorous_Plant_Nursery.Repositories;
using Carnivorous_Plant_Nursery.Models;

namespace Carnivorous_Plant_Nursery.Controllers
{
    public class InventoryController : Controller
    {
        private readonly InventoryMockRepository _inventoryRepository;

        public InventoryController(InventoryMockRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public IActionResult Index()
        {
            var inventory = _inventoryRepository.GetWebshopItems();
            return View(inventory);
        }

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
