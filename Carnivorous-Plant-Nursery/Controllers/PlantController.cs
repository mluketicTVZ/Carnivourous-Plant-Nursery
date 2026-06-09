using Microsoft.AspNetCore.Mvc;
using Carnivorous_Plant_Nursery.Repositories;
using Carnivorous_Plant_Nursery.Models;
using Microsoft.AspNetCore.Authorization;

namespace Carnivorous_Plant_Nursery.Controllers
{
    [Route("plants")]
    public class PlantController : BaseController
    {
        private readonly PlantRepository _plantRepository;
        private readonly TaxonomyRepository _taxonomyRepository;
        private readonly SeedBatchRepository _seedBatchRepository;
        private readonly AttachmentRepository _attachmentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const long MaxAttachmentSizeBytes = 5 * 1024 * 1024;
        private const string PendingAttachmentFolder = "_pending";
        private static readonly HashSet<string> AllowedAttachmentExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp",
            ".gif"
        };

        public PlantController(
            PlantRepository plantRepository,
            TaxonomyRepository taxonomyRepository,
            SeedBatchRepository seedBatchRepository,
            AttachmentRepository attachmentRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _plantRepository = plantRepository;
            _taxonomyRepository = taxonomyRepository;
            _seedBatchRepository = seedBatchRepository;
            _attachmentRepository = attachmentRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("")]
        public async Task<IActionResult> Index([FromQuery] string searchTerm, [FromQuery] bool? webshopOnly, [FromQuery] PlantStage? stage, [FromQuery] HealthState? healthStatus)
        {
            var plants = await _plantRepository.Search(searchTerm, webshopOnly, stage, healthStatus);

            ViewBag.SearchTerm = searchTerm;
            ViewBag.WebshopOnly = webshopOnly;
            ViewBag.Stage = stage;
            ViewBag.HealthStatus = healthStatus;

            return View(plants);
        }

        [Route("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var plant = await _plantRepository.GetById(id);
            if (plant == null)
                return NotFound();
            return View(plant);
        }

        [HttpGet]
        [Route("create")]
        [Authorize(Roles = AuthorizationRole.AdminOrManager)]
        public async Task<IActionResult> Create()
        {
            ViewBag.Taxonomies = await _taxonomyRepository.GetAll();
            ViewBag.SeedBatches = await _seedBatchRepository.GetAll();
            return View();
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRole.AdminOrManager)]
        public async Task<IActionResult> Create(Plant model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Taxonomies = await _taxonomyRepository.GetAll();
                ViewBag.SeedBatches = await _seedBatchRepository.GetAll();
                return View(model);
            }
            await _plantRepository.Add(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("edit/{id:int}")]
        [ActionName("Edit")]
        [Authorize(Roles = AuthorizationRole.AdminOrManager)]
        public async Task<IActionResult> EditGet(int id)
        {
            var plant = await _plantRepository.GetById(id);
            if (plant == null) return NotFound();
            ViewBag.Taxonomies = await _taxonomyRepository.GetAll();
            ViewBag.SeedBatches = await _seedBatchRepository.GetAll();
            return View(plant);
        }

        [HttpPost]
        [Route("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        [Authorize(Roles = AuthorizationRole.AdminOrManager)]
        public async Task<IActionResult> EditPost(int id)
        {
            var entity = await _plantRepository.GetById(id);
            if (entity == null) return NotFound();
            var pendingAttachments = ReadPendingAttachments(id);
            var removedAttachmentIds = ReadRemovedAttachmentIds();

            if (!PendingAttachmentFilesExist(id, pendingAttachments))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage.AttachmentPendingFileMissing);
            }

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
                e => e.CurrentStage,
                e => e.PotDiameterCm,
                e => e.PotHeightCm,
                e => e.LastRepottingDate,
                e => e.LastDormancyDateStart,
                e => e.LastDormancyDateEnd,
                e => e.EstimatedAgeAtAcquiryYears,
                e => e.HealthStatus,
                e => e.HealthDescription,
                e => e.SourceSeedBatchId))
            {
                ViewBag.Taxonomies = await _taxonomyRepository.GetAll();
                ViewBag.SeedBatches = await _seedBatchRepository.GetAll();
                return View(entity);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Taxonomies = await _taxonomyRepository.GetAll();
                ViewBag.SeedBatches = await _seedBatchRepository.GetAll();
                return View(entity);
            }

            await _plantRepository.Update(entity);
            await CommitAttachmentChanges(id, pendingAttachments, removedAttachmentIds);
            return RedirectToAction("Details", new { id });
        }

        [HttpGet]
        [Route("{id:int}/attachments")]
        [Authorize(Roles = AuthorizationRole.AdminOrManager)]
        public async Task<IActionResult> Attachments(int id)
        {
            var plant = await _plantRepository.GetById(id);
            if (plant == null)
                return NotFound(ErrorMessage.AttachmentPlantNotFound);

            var attachments = await _attachmentRepository.GetActiveForPlant(id);
            ViewData["PlantId"] = id;
            return PartialView("_PlantAttachmentList", attachments);
        }

        [HttpPost]
        [Route("{id:int}/attachments/upload")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRole.AdminOrManager)]
        public async Task<IActionResult> UploadAttachment(int id, IFormFile file)
        {
            var plant = await _plantRepository.GetById(id);
            if (plant == null)
                return NotFound(new { error = ErrorMessage.AttachmentPlantNotFound });

            var validationError = ValidateAttachment(file);
            if (validationError != null)
                return BadRequest(new { error = validationError });

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var storedFileName = $"{Guid.NewGuid():N}{extension}";
            var uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "plants", PendingAttachmentFolder, id.ToString());
            Directory.CreateDirectory(uploadDirectory);

            var physicalPath = Path.Combine(uploadDirectory, storedFileName);
            await using (var stream = new FileStream(physicalPath, FileMode.CreateNew))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new
            {
                success = true,
                fileName = Path.GetFileName(file.FileName),
                storedFileName,
                filePath = $"/uploads/plants/{PendingAttachmentFolder}/{id}/{storedFileName}",
                contentType = file.ContentType,
                fileSize = file.Length
            });
        }

        [HttpPost]
        [Route("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRole.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _plantRepository.Delete(id);
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
            if (string.IsNullOrWhiteSpace(term) || term.Trim().Length < 1)
                return Json(new List<object>());

            var plants = await _plantRepository.Search(term.Trim());
            var lowerTerm = term.Trim().ToLower();

            var results = plants
                .Where(p => p.Taxonomy != null && !string.IsNullOrEmpty(p.Taxonomy.CommonName))
                .Select(p => new
                {
                    text = string.IsNullOrEmpty(p.Taxonomy!.Cultivar)
                        ? $"{p.Taxonomy.CommonName} ({p.Taxonomy.Genus} {p.Taxonomy.Species})"
                        : $"{p.Taxonomy.CommonName} ({p.Taxonomy.Genus} {p.Taxonomy.Species} '{p.Taxonomy.Cultivar}')",
                    value = p.Taxonomy.CommonName!
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

        private static string? ValidateAttachment(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return ErrorMessage.AttachmentFileRequired;

            var extension = Path.GetExtension(file.FileName);
            if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) ||
                !AllowedAttachmentExtensions.Contains(extension))
            {
                return ErrorMessage.AttachmentInvalidImageType;
            }

            if (file.Length > MaxAttachmentSizeBytes)
                return ErrorMessage.AttachmentTooLarge;

            return null;
        }

        private List<PendingAttachment> ReadPendingAttachments(int plantId)
        {
            var storedFileNames = Request.Form["PendingAttachmentStoredFileNames"];
            var fileNames = Request.Form["PendingAttachmentFileNames"];
            var contentTypes = Request.Form["PendingAttachmentContentTypes"];
            var fileSizes = Request.Form["PendingAttachmentFileSizes"];
            var pendingAttachments = new List<PendingAttachment>();

            for (var index = 0; index < storedFileNames.Count; index++)
            {
                var storedFileName = Path.GetFileName(storedFileNames[index]);
                var extension = Path.GetExtension(storedFileName);
                if (string.IsNullOrWhiteSpace(storedFileName) ||
                    string.IsNullOrWhiteSpace(extension) ||
                    !AllowedAttachmentExtensions.Contains(extension))
                {
                    continue;
                }

                var fileName = index < fileNames.Count ? Path.GetFileName(fileNames[index]) : storedFileName;
                var contentType = index < contentTypes.Count ? contentTypes[index] ?? string.Empty : string.Empty;
                _ = long.TryParse(index < fileSizes.Count ? fileSizes[index] : null, out var fileSize);

                pendingAttachments.Add(new PendingAttachment(
                    storedFileName,
                    string.IsNullOrWhiteSpace(fileName) ? storedFileName : fileName,
                    string.IsNullOrWhiteSpace(contentType) ? "image/*" : contentType,
                    fileSize,
                    GetPendingAttachmentPath(plantId, storedFileName)));
            }

            return pendingAttachments;
        }

        private List<int> ReadRemovedAttachmentIds() =>
            Request.Form["RemovedAttachmentIds"]
                .Select(value => int.TryParse(value, out var id) ? id : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();

        private bool PendingAttachmentFilesExist(int plantId, IEnumerable<PendingAttachment> pendingAttachments)
        {
            return pendingAttachments.All(attachment =>
                attachment.PhysicalPath.StartsWith(GetPendingAttachmentDirectory(plantId), StringComparison.OrdinalIgnoreCase) &&
                System.IO.File.Exists(attachment.PhysicalPath));
        }

        private async Task CommitAttachmentChanges(int plantId, List<PendingAttachment> pendingAttachments, List<int> removedAttachmentIds)
        {
            await _attachmentRepository.SoftDeleteMany(plantId, removedAttachmentIds);

            if (pendingAttachments.Count == 0)
                return;

            var finalDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "plants", plantId.ToString());
            Directory.CreateDirectory(finalDirectory);

            var attachments = new List<Attachment>();
            foreach (var pendingAttachment in pendingAttachments)
            {
                var finalPath = Path.Combine(finalDirectory, pendingAttachment.StoredFileName);
                System.IO.File.Move(pendingAttachment.PhysicalPath, finalPath, overwrite: false);

                attachments.Add(new Attachment
                {
                    PlantId = plantId,
                    FileName = pendingAttachment.FileName,
                    StoredFileName = pendingAttachment.StoredFileName,
                    FilePath = $"/uploads/plants/{plantId}/{pendingAttachment.StoredFileName}",
                    ContentType = pendingAttachment.ContentType,
                    FileSize = pendingAttachment.FileSize,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _attachmentRepository.AddRange(attachments);
        }

        private string GetPendingAttachmentDirectory(int plantId) =>
            Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "plants", PendingAttachmentFolder, plantId.ToString());

        private string GetPendingAttachmentPath(int plantId, string storedFileName) =>
            Path.Combine(GetPendingAttachmentDirectory(plantId), storedFileName);

        private sealed record PendingAttachment(
            string StoredFileName,
            string FileName,
            string ContentType,
            long FileSize,
            string PhysicalPath);
    }
}
