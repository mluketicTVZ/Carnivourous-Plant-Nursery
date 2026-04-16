using Microsoft.AspNetCore.Mvc;
using Carnivorous_Plant_Nursery.Repositories;

namespace Carnivorous_Plant_Nursery.Controllers
{
    public class CareProfileController : Controller
    {
        private readonly CareProfileMockRepository _careProfileRepository;

        public CareProfileController(CareProfileMockRepository careProfileRepository)
        {
            _careProfileRepository = careProfileRepository;
        }

        public IActionResult Index()
        {
            var careProfiles = _careProfileRepository.GetAll();
            return View(careProfiles);
        }

        public IActionResult Details(int id)
        {
            var careProfile = _careProfileRepository.GetById(id);
            if (careProfile == null)
            {
                return NotFound();
            }
            return View(careProfile);
        }
    }
}
