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
        public async Task<IActionResult> Index([FromQuery] string searchTerm, [FromQuery] bool? webshopOnly, [FromQuery] PlantStage? stage, [FromQuery] HealthState? healthStatus)
        {
            var plants = await _plantRepository.Search(searchTerm);

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
        public async Task<IActionResult> Details(int id)
        {
            var plant = await _plantRepository.GetById(id);
            if (plant == null)
                return NotFound();
            return View(plant);
        }

        [HttpGet]
        [Route("create")]
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin) return RequireAdmin();
            ViewBag.Taxonomies = await _taxonomyRepository.GetAll();
            return View();
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Plant model)
        {
            if (!IsAdmin) return RequireAdmin();
            if (!ModelState.IsValid)
            {
                ViewBag.Taxonomies = await _taxonomyRepository.GetAll();
                return View(model);
            }
            await _plantRepository.Add(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("edit/{id:int}")]
        [ActionName("Edit")]
        public async Task<IActionResult> EditGet(int id)
        {
            if (!IsAdmin) return RequireAdmin();
            var plant = await _plantRepository.GetById(id);
            if (plant == null) return NotFound();
            ViewBag.Taxonomies = await _taxonomyRepository.GetAll();
            return View(plant);
        }

        [HttpPost]
        [Route("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPost(int id)
        {
            if (!IsAdmin) return RequireAdmin();

            var entity = await _plantRepository.GetById(id);
            if (entity == null) return NotFound();

            if (!await TryUpdateModelAsync(entity, "",
                e => e.SKU,
                e => e.ListingTitle,
                e => e.Price,
                e => e.IsAvailableInWebshop,
                e => e.Description,
                e => e.TaxonomyId,
                e => e.LineageId,
                e => e.DateAcquired,
                e => e.InternalNotes,
                e => e.LocationInNursery,
                e => e.CurrentStage,
                e => e.PotDiameterCm,
                e => e.PotHeightCm,
                e => e.LastRepottingDate,
                e => e.LastDormancyDateStart,
                e => e.LastDormancyDateEnd,
                e => e.EstimatedAgeAtAcquiryYears,
                e => e.HealthStatus,
                e => e.HealthDescription))
            {
                ViewBag.Taxonomies = await _taxonomyRepository.GetAll();
                return View(entity);
            }

            await _plantRepository.Update(entity);
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [Route("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin) return RequireAdmin();
            try
            {
                await _plantRepository.Delete(id);
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

