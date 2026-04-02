using System.Diagnostics;
using Carnivorous_Plant_Nursery.Models;
using Carnivorous_Plant_Nursery.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Carnivorous_Plant_Nursery.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly InventorySimulationService _inventorySimulator;

        public HomeController(ILogger<HomeController> logger, InventorySimulationService inventorySimulator)
        {
            _logger = logger;
            _inventorySimulator = inventorySimulator;
        }

        public async Task<IActionResult> Index()
        {
            var taxonomies = await _inventorySimulator.FetchInventoryAsync();
            var model = new HomeIndexViewModel
            {
                Taxonomies = taxonomies,
                WebshopItems = _inventorySimulator.GetWebshopItems(taxonomies),
                SeedsForStratification = _inventorySimulator.GetSeedsRequiringStratification(taxonomies),
                LineagePlants = _inventorySimulator.GetItemsWithKnownLineage(taxonomies)
            };
            
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
