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
        public IActionResult Index()
        {
            var seedBatches = _seedBatchRepository.GetAll();
            return View(seedBatches);
        }

        [Route("{id:int}")]
        public IActionResult Details(int id)
        {
            var seedBatch = _seedBatchRepository.GetById(id);
            if (seedBatch == null)
                return NotFound();
            return View(seedBatch);
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
        public IActionResult Create(SeedBatch model)
        {
            if (!IsAdmin) return RequireAdmin();
            if (!ModelState.IsValid)
            {
                ViewBag.Taxonomies = _taxonomyRepository.GetAll();
                return View(model);
            }
            _seedBatchRepository.Add(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("edit/{id:int}")]
        public IActionResult Edit(int id)
        {
            if (!IsAdmin) return RequireAdmin();
            var seedBatch = _seedBatchRepository.GetById(id);
            if (seedBatch == null) return NotFound();
            ViewBag.Taxonomies = _taxonomyRepository.GetAll();
            return View(seedBatch);
        }

        [HttpPost]
        [Route("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, SeedBatch model)
        {
            if (!IsAdmin) return RequireAdmin();
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewBag.Taxonomies = _taxonomyRepository.GetAll();
                return View(model);
            }
            _seedBatchRepository.Update(model);
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
                _seedBatchRepository.Delete(id);
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
