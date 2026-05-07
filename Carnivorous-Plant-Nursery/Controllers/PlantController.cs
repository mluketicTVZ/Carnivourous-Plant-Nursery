using Microsoft.AspNetCore.Mvc;
using Carnivorous_Plant_Nursery.Repositories;
using Carnivorous_Plant_Nursery.Models;

namespace Carnivorous_Plant_Nursery.Controllers
{
    [Route("plants")]
    public class PlantController : BaseController
    {
        private readonly PlantRepository _plantRepository;
        private readonly TaxonomyRepository _taxonomyRepository;

        public PlantController(PlantRepository plantRepository, TaxonomyRepository taxonomyRepository)
        {
            _plantRepository = plantRepository;
            _taxonomyRepository = taxonomyRepository;
        }

        [Route("")]
        public IActionResult Index([FromQuery] string searchTerm, [FromQuery] bool? webshopOnly, [FromQuery] PlantStage? stage, [FromQuery] HealthState? healthStatus)
        {
            var plants = _plantRepository.Search(searchTerm);

            if (webshopOnly == true)
                plants = plants.Where(p => p.IsAvailableInWebshop).ToList();

            if (stage.HasValue)
                plants = plants.Where(p => p.CurrentStage == stage.Value).ToList();

            if (healthStatus.HasValue)
                plants = plants.Where(p => p.HealthStatus == healthStatus.Value).ToList();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.WebshopOnly = webshopOnly;
            ViewBag.Stage = stage;
            ViewBag.HealthStatus = healthStatus;

            return View(plants);
        }

        [Route("{id:int}")]
        public IActionResult Details(int id)
        {
            var plant = _plantRepository.GetById(id);
            if (plant == null)
                return NotFound();
            return View(plant);
        }

        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            if (!IsAdmin) return RequireAdmin();
            ViewBag.Taxonomies = _taxonomyRepository.GetAll();
            return View();
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Plant model)
        {
            if (!IsAdmin) return RequireAdmin();
            if (!ModelState.IsValid)
            {
                ViewBag.Taxonomies = _taxonomyRepository.GetAll();
                return View(model);
            }
            _plantRepository.Add(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("edit/{id:int}")]
        public IActionResult Edit(int id)
        {
            if (!IsAdmin) return RequireAdmin();
            var plant = _plantRepository.GetById(id);
            if (plant == null) return NotFound();
            ViewBag.Taxonomies = _taxonomyRepository.GetAll();
            return View(plant);
        }

        [HttpPost]
        [Route("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Plant model)
        {
            if (!IsAdmin) return RequireAdmin();
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewBag.Taxonomies = _taxonomyRepository.GetAll();
                return View(model);
            }
            _plantRepository.Update(model);
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [Route("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (!IsAdmin) return RequireAdmin();
            try
            {
                _plantRepository.Delete(id);
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                TempData["DeleteError"] = ex.Message;
                return RedirectToAction("Details", new { id });
            }
        }
    }
}

