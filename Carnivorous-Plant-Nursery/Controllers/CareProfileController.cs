using Microsoft.AspNetCore.Mvc;
using Carnivorous_Plant_Nursery.Repositories;
using Carnivorous_Plant_Nursery.Models;

namespace Carnivorous_Plant_Nursery.Controllers
{
    [Route("care")]
    public class CareProfileController : BaseController
    {
        private readonly CareProfileRepository _careProfileRepository;

        public CareProfileController(CareProfileRepository careProfileRepository)
        {
            _careProfileRepository = careProfileRepository;
        }

        [Route("")]
        public IActionResult Index()
        {
            var careProfiles = _careProfileRepository.GetAll();
            return View(careProfiles);
        }

        [Route("{id:int}")]
        public IActionResult Details(int id)
        {
            var careProfile = _careProfileRepository.GetById(id);
            if (careProfile == null)
                return NotFound();
            return View(careProfile);
        }

        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            if (!IsAdmin) return RequireAdmin();
            return View();
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CareProfile model)
        {
            if (!IsAdmin) return RequireAdmin();
            if (!ModelState.IsValid) return View(model);
            _careProfileRepository.Add(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("edit/{id:int}")]
        public IActionResult Edit(int id)
        {
            if (!IsAdmin) return RequireAdmin();
            var careProfile = _careProfileRepository.GetById(id);
            if (careProfile == null) return NotFound();
            return View(careProfile);
        }

        [HttpPost]
        [Route("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CareProfile model)
        {
            if (!IsAdmin) return RequireAdmin();
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);
            _careProfileRepository.Update(model);
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
                _careProfileRepository.Delete(id);
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
