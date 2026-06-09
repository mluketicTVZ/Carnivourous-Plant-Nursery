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
        public async Task<IActionResult> Index(string? searchTerm, bool? availableInWebshop)
        {
            var items = await _seedBatchRepository.Search(searchTerm, availableInWebshop);

            ViewBag.SearchTerm = searchTerm;
            ViewBag.AvailableInWebshop = availableInWebshop;
            return View(items);
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

        [HttpGet]
        [Route("suggestions")]
        public async Task<IActionResult> Suggestions([FromQuery] string? term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(Array.Empty<object>());

            var all = await _seedBatchRepository.GetAll();
            var lowerTerm = term.ToLowerInvariant();

            var results = all
                .Where(sb => !string.IsNullOrEmpty(sb.ListingTitle) &&
                             (sb.ListingTitle.Contains(lowerTerm, StringComparison.OrdinalIgnoreCase) ||
                              (!string.IsNullOrEmpty(sb.SKU) && sb.SKU.Contains(lowerTerm, StringComparison.OrdinalIgnoreCase)) ||
                              (sb.Taxonomy?.CommonName != null && sb.Taxonomy.CommonName.Contains(lowerTerm, StringComparison.OrdinalIgnoreCase)) ||
                              sb.Taxonomy?.FullName.Contains(lowerTerm, StringComparison.OrdinalIgnoreCase) == true))
                .Select(sb => new
                {
                    text = string.IsNullOrEmpty(sb.SKU) ? sb.ListingTitle! : $"{sb.ListingTitle} [{sb.SKU}]",
                    value = sb.ListingTitle!
                })
                .DistinctBy(x => x.value)
                .OrderBy(x =>
                {
                    var idx = x.text.IndexOf(lowerTerm, StringComparison.OrdinalIgnoreCase);
                    return idx >= 0 ? idx : int.MaxValue;
                })
                .Take(8)
                .ToList();

            return Json(results);
        }

        [HttpGet]
        [Route("batch-suggestions")]
        public async Task<IActionResult> BatchSuggestions([FromQuery] string? term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(Array.Empty<object>());

            var all = await _seedBatchRepository.GetAll();
            var lowerTerm = term.ToLowerInvariant();

            var results = all
                .Where(sb => !string.IsNullOrEmpty(sb.ListingTitle) &&
                             (sb.ListingTitle.Contains(lowerTerm, StringComparison.OrdinalIgnoreCase) ||
                              (!string.IsNullOrEmpty(sb.SKU) && sb.SKU.Contains(lowerTerm, StringComparison.OrdinalIgnoreCase)) ||
                              (sb.Taxonomy?.CommonName != null && sb.Taxonomy.CommonName.Contains(lowerTerm, StringComparison.OrdinalIgnoreCase)) ||
                              sb.Taxonomy?.FullName.Contains(lowerTerm, StringComparison.OrdinalIgnoreCase) == true))
                .Select(sb => new
                {
                    text = string.IsNullOrEmpty(sb.SKU) ? sb.ListingTitle! : $"{sb.ListingTitle} [{sb.SKU}]",
                    value = sb.Id.ToString()
                })
                .DistinctBy(x => x.value)
                .OrderBy(x =>
                {
                    var idx = x.text.IndexOf(lowerTerm, StringComparison.OrdinalIgnoreCase);
                    return idx >= 0 ? idx : int.MaxValue;
                })
                .Take(8)
                .ToList();

            return Json(results);
        }
    }
}
