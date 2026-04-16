using Microsoft.AspNetCore.Mvc;
using Carnivorous_Plant_Nursery.Repositories;
using Carnivorous_Plant_Nursery.Models;

namespace Carnivorous_Plant_Nursery.Controllers
{
    public class PlantController : Controller
    {
        private readonly PlantMockRepository _plantRepository;

        public PlantController(PlantMockRepository plantRepository)
        {
            _plantRepository = plantRepository;
        }

        public IActionResult Index()
        {
            var plants = _plantRepository.GetAll();
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
