using Microsoft.AspNetCore.Mvc;
using Carnivorous_Plant_Nursery.Repositories;
using Carnivorous_Plant_Nursery.Models;

namespace Carnivorous_Plant_Nursery.Controllers
{
    [Route("taxonomy")]
    public class TaxonomyController : BaseController
    {
        private readonly TaxonomyRepository _taxonomyRepository;
        private readonly CareProfileRepository _careProfileRepository;

        public TaxonomyController(TaxonomyRepository taxonomyRepository, CareProfileRepository careProfileRepository)
        {
            _taxonomyRepository = taxonomyRepository;
            _careProfileRepository = careProfileRepository;
        }

        [Route("")]
        public IActionResult Index(string? searchTerm)
        {
            var taxonomies = _taxonomyRepository.Search(searchTerm ?? string.Empty);
            ViewData["SearchTerm"] = searchTerm;
            return View(taxonomies);
        }

        [Route("{id:int}")]
        public IActionResult Details(int id)
        {
            var taxonomy = _taxonomyRepository.GetById(id);
            if (taxonomy == null)
                return NotFound();
            return View(taxonomy);
        }

        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            if (!IsAdmin) return RequireAdmin();
            ViewBag.CareProfiles = _careProfileRepository.GetAll();
            return View();
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Taxonomy model)
        {
            if (!IsAdmin) return RequireAdmin();
            if (!ModelState.IsValid)
            {
                ViewBag.CareProfiles = _careProfileRepository.GetAll();
                return View(model);
            }
            _taxonomyRepository.Add(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("edit/{id:int}")]
        public IActionResult Edit(int id)
        {
            if (!IsAdmin) return RequireAdmin();
            var taxonomy = _taxonomyRepository.GetById(id);
            if (taxonomy == null) return NotFound();
            ViewBag.CareProfiles = _careProfileRepository.GetAll();
            return View(taxonomy);
        }

        [HttpPost]
        [Route("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Taxonomy model)
        {
            if (!IsAdmin) return RequireAdmin();
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewBag.CareProfiles = _careProfileRepository.GetAll();
                return View(model);
            }
            _taxonomyRepository.Update(model);
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
                _taxonomyRepository.Delete(id);
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
