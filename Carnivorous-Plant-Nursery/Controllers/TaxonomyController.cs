using Microsoft.AspNetCore.Mvc;
using Carnivorous_Plant_Nursery.Repositories;

namespace Carnivorous_Plant_Nursery.Controllers
{
    public class TaxonomyController : Controller
    {
        private readonly TaxonomyMockRepository _taxonomyRepository;

        public TaxonomyController(TaxonomyMockRepository taxonomyRepository)
        {
            _taxonomyRepository = taxonomyRepository;
        }

        public IActionResult Index(string? searchTerm)
        {
            var taxonomies = _taxonomyRepository.Search(searchTerm ?? string.Empty);
            ViewData["SearchTerm"] = searchTerm;
            return View(taxonomies);
        }

        public IActionResult Details(int id)
        {
            var taxonomy = _taxonomyRepository.GetById(id);
            if (taxonomy == null)
            {
                return NotFound();
            }
            return View(taxonomy);
        }
    }
}
