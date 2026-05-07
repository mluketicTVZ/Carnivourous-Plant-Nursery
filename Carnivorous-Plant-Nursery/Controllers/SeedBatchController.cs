using Microsoft.AspNetCore.Mvc;
using Carnivorous_Plant_Nursery.Repositories;

namespace Carnivorous_Plant_Nursery.Controllers
{
    public class SeedBatchController : Controller
    {
        private readonly SeedBatchRepository _seedBatchRepository;

        public SeedBatchController(SeedBatchRepository seedBatchRepository)
        {
            _seedBatchRepository = seedBatchRepository;
        }

        public IActionResult Index()
        {
            var seedBatches = _seedBatchRepository.GetAll();
            return View(seedBatches);
        }

        public IActionResult Details(int id)
        {
            var seedBatch = _seedBatchRepository.GetById(id);
            if (seedBatch == null)
            {
                return NotFound();
            }
            return View(seedBatch);
        }
    }
}
