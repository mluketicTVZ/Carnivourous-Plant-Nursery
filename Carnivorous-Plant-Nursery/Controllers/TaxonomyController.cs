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
        public async Task<IActionResult> Index(string? searchTerm)
        {
            var taxonomies = await _taxonomyRepository.Search(searchTerm ?? string.Empty);
            ViewData["SearchTerm"] = searchTerm;
            return View(taxonomies);
        }

        [Route("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var taxonomy = await _taxonomyRepository.GetById(id);
            if (taxonomy == null)
                return NotFound();
            return View(taxonomy);
        }

        [HttpGet]
        [Route("create")]
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin) return RequireAdmin();
            ViewBag.CareProfiles = await _careProfileRepository.GetAll();
            return View();
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Taxonomy model)
        {
            if (!IsAdmin) return RequireAdmin();
            if (!ModelState.IsValid)
            {
                ViewBag.CareProfiles = await _careProfileRepository.GetAll();
                return View(model);
            }
            await _taxonomyRepository.Add(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("edit/{id:int}")]
        [ActionName("Edit")]
        public async Task<IActionResult> EditGet(int id)
        {
            if (!IsAdmin) return RequireAdmin();
            var taxonomy = await _taxonomyRepository.GetById(id);
            if (taxonomy == null) return NotFound();
            ViewBag.CareProfiles = await _careProfileRepository.GetAll();
            return View(taxonomy);
        }

        [HttpPost]
        [Route("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPost(int id)
        {
            if (!IsAdmin) return RequireAdmin();

            var entity = await _taxonomyRepository.GetById(id);
            if (entity == null) return NotFound();

            if (!await TryUpdateModelAsync(entity, "",
                e => e.Genus,
                e => e.Species,
                e => e.Cultivar,
                e => e.CommonName,
                e => e.CareProfileId))
            {
                ViewBag.CareProfiles = await _careProfileRepository.GetAll();
                return View(entity);
            }

            await _taxonomyRepository.Update(entity);
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
                await _taxonomyRepository.Delete(id);
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                TempData["DeleteError"] = ex.Message;
                return RedirectToAction("Details", new { id });
            }
        }

        [HttpGet]
        [Route("suggestions")]
        public async Task<IActionResult> Suggestions([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(Array.Empty<object>());

            var taxonomies = await _taxonomyRepository.Search(term.Trim());
            var lowerTerm = term.Trim().ToLower();

            var results = taxonomies
                .Where(t => !string.IsNullOrEmpty(t.CommonName))
                .Select(t => new
                {
                    text = string.IsNullOrEmpty(t.Cultivar)
                        ? $"{t.CommonName} ({t.Genus} {t.Species})"
                        : $"{t.CommonName} ({t.Genus} {t.Species} '{t.Cultivar}')",
                    value = t.CommonName!
                })
                .DistinctBy(x => x.value)
                .OrderBy(x =>
                {
                    var idx = x.text.ToLower().IndexOf(lowerTerm);
                    return idx < 0 ? int.MaxValue : idx;
                })
                .Take(8)
                .ToList();

            return Json(results);
        }

        // Returns taxonomy suggestions with ID as value — used by Plant/SeedBatch Create/Edit FK pickers
        [HttpGet]
        [Route("id-suggestions")]
        public async Task<IActionResult> IdSuggestions([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(Array.Empty<object>());

            var taxonomies = await _taxonomyRepository.Search(term.Trim());
            var lowerTerm = term.Trim().ToLower();

            var results = taxonomies
                .Where(t => !string.IsNullOrEmpty(t.CommonName))
                .Select(t => new
                {
                    text = string.IsNullOrEmpty(t.Cultivar)
                        ? $"{t.CommonName} ({t.Genus} {t.Species})"
                        : $"{t.CommonName} ({t.Genus} {t.Species} '{t.Cultivar}')",
                    value = t.Id.ToString()
                })
                .OrderBy(x =>
                {
                    var idx = x.text.ToLower().IndexOf(lowerTerm);
                    return idx < 0 ? int.MaxValue : idx;
                })
                .Take(8)
                .ToList();

            return Json(results);
        }
    }
}
