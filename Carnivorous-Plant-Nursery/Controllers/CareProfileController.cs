using Microsoft.AspNetCore.Mvc;
using Carnivorous_Plant_Nursery.Repositories;
using Carnivorous_Plant_Nursery.Models;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> Index(string? searchTerm, string? requiredLight)
        {
            var allCareProfiles = await _careProfileRepository.GetAll();

            // Populate dropdown options BEFORE filtering so all names are always shown
            ViewBag.AllCareProfiles = allCareProfiles
                .Select(cp => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = cp.CareProfileName,
                    Value = cp.CareProfileName
                })
                .ToList();

            IEnumerable<CareProfile> careProfiles = allCareProfiles;

            if (!string.IsNullOrWhiteSpace(searchTerm))
                careProfiles = careProfiles
                    .Where(cp => cp.CareProfileName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(requiredLight) && Enum.TryParse<LightLevel>(requiredLight, out var parsedLight))
                careProfiles = careProfiles
                    .Where(cp => cp.RequiredLight == parsedLight);

            ViewBag.SearchTerm = searchTerm;
            ViewBag.RequiredLight = requiredLight;

            return View(careProfiles.ToList());
        }

        [Route("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var careProfile = await _careProfileRepository.GetById(id);
            if (careProfile == null)
                return NotFound();
            return View(careProfile);
        }

        [HttpGet]
        [Route("create")]
        [Authorize(Roles = AuthorizationRole.AdminOrManager)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRole.AdminOrManager)]
        public async Task<IActionResult> Create(CareProfile model)
        {
            if (!ModelState.IsValid) return View(model);
            await _careProfileRepository.Add(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("edit/{id:int}")]
        [ActionName("Edit")]
        [Authorize(Roles = AuthorizationRole.AdminOrManager)]
        public async Task<IActionResult> EditGet(int id)
        {
            var careProfile = await _careProfileRepository.GetById(id);
            if (careProfile == null) return NotFound();
            return View(careProfile);
        }

        [HttpPost]
        [Route("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        [Authorize(Roles = AuthorizationRole.AdminOrManager)]
        public async Task<IActionResult> EditPost(int id)
        {
            var entity = await _careProfileRepository.GetById(id);
            if (entity == null) return NotFound();

            if (!await TryUpdateModelAsync(entity, "",
                e => e.CareProfileName,
                e => e.RequiredLight,
                e => e.MinTemperature,
                e => e.MaxTemperature,
                e => e.TemperatureDescription,
                e => e.RequiresWinterDormancy,
                e => e.SoilMix,
                e => e.RequiredHumidity,
                e => e.CareDescription))
            {
                return View(entity);
            }

            await _careProfileRepository.Update(entity);
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [Route("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRole.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _careProfileRepository.Delete(id);
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
