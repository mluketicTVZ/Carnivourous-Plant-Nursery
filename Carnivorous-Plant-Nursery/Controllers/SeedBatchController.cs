using Microsoft.AspNetCore.Mvc;
using Carnivorous_Plant_Nursery.Repositories;
using Carnivorous_Plant_Nursery.Models;

namespace Carnivorous_Plant_Nursery.Controllers
{
    [Route("seeds")]
    public class SeedBatchController : BaseController
    {
        private readonly SeedBatchRepository _seedBatchRepository;
        private readonly TaxonomyRepository _taxonomyRepository;

        public SeedBatchController(SeedBatchRepository seedBatchRepository, TaxonomyRepository taxonomyRepository)
        {
            _seedBatchRepository = seedBatchRepository;
            _taxonomyRepository = taxonomyRepository;
        }

        [Route("")]
        public async Task<IActionResult> Index()
        {
            var seedBatches = await _seedBatchRepository.GetAll();
            return View(seedBatches);
        }

        [Route("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var seedBatch = await _seedBatchRepository.GetById(id);
            if (seedBatch == null)
                return NotFound();
            return View(seedBatch);
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
        public async Task<IActionResult> Create(SeedBatch model)
        {
            if (!IsAdmin) return RequireAdmin();
            if (!ModelState.IsValid)
            {
                ViewBag.Taxonomies = await _taxonomyRepository.GetAll();
                return View(model);
            }
            await _seedBatchRepository.Add(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("edit/{id:int}")]
        [ActionName("Edit")]
        public async Task<IActionResult> EditGet(int id)
        {
            if (!IsAdmin) return RequireAdmin();
            var seedBatch = await _seedBatchRepository.GetById(id);
            if (seedBatch == null) return NotFound();
            ViewBag.Taxonomies = await _taxonomyRepository.GetAll();
            return View(seedBatch);
        }

        [HttpPost]
        [Route("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPost(int id)
        {
            if (!IsAdmin) return RequireAdmin();

            var entity = await _seedBatchRepository.GetById(id);
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
                e => e.SeedCount,
                e => e.HarvestDate,
                e => e.ExpectedViabilityMonths,
                e => e.RequiresStratification,
                e => e.EstimatedGerminationRate))
            {
                ViewBag.Taxonomies = await _taxonomyRepository.GetAll();
                return View(entity);
            }

            await _seedBatchRepository.Update(entity);
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
                await _seedBatchRepository.Delete(id);
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
