using Microsoft.AspNetCore.Mvc;
using Carnivorous_Plant_Nursery.Repositories;
using Carnivorous_Plant_Nursery.Models;

namespace Carnivorous_Plant_Nursery.Controllers
{
    public class PlantController : Controller
    {
        private readonly PlantRepository _plantRepository;

        public PlantController(PlantRepository plantRepository)
        {
            _plantRepository = plantRepository;
        }

        public IActionResult Index([FromQuery] string searchTerm, [FromQuery] bool? webshopOnly, [FromQuery] PlantStage? stage, [FromQuery] HealthState? healthStatus)
        {
            var plants = _plantRepository.Search(searchTerm);

            if (webshopOnly == true)
            {
                plants = plants.Where(p => p.IsAvailableInWebshop).ToList();
            }

            if (stage.HasValue)
            {
                plants = plants.Where(p => p.CurrentStage == stage.Value).ToList();
            }

            if (healthStatus.HasValue)
            {
                plants = plants.Where(p => p.HealthStatus == healthStatus.Value).ToList();
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.WebshopOnly = webshopOnly;
            ViewBag.Stage = stage;
            ViewBag.HealthStatus = healthStatus;

            return View(plants);
        }

        public IActionResult Details(int id)
        {
            var plant = _plantRepository.GetById(id);
            if (plant == null)
            {
                return NotFound();
            }
            return View(plant);
        }
    }
}
