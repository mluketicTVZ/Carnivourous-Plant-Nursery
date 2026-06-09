using Microsoft.AspNetCore.Mvc;
using Carnivorous_Plant_Nursery.Repositories;
using Carnivorous_Plant_Nursery.Models;
using Microsoft.AspNetCore.Authorization;

namespace Carnivorous_Plant_Nursery.Controllers
{
    [Route("seeds")]
    public class SeedBatchController : BaseController
    {
        private readonly SeedBatchRepository _seedBatchRepository;
        private readonly TaxonomyRepository _taxonomyRepository;
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

        public SeedBatchController(
            SeedBatchRepository seedBatchRepository,
            TaxonomyRepository taxonomyRepository,
            AttachmentRepository attachmentRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _seedBatchRepository = seedBatchRepository;
            _taxonomyRepository = taxonomyRepository;
            _attachmentRepository = attachmentRepository;
            _webHostEnvironment = webHostEnvironment;
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
        [Authorize(Roles = AuthorizationRole.AdminOrManager)]
        public async Task<IActionResult> Create()
        {
            ViewBag.Taxonomies = await _taxonomyRepository.GetAll();
            return View();
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRole.AdminOrManager)]
        public async Task<IActionResult> Create(SeedBatch model)
        {
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
        [Authorize(Roles = AuthorizationRole.AdminOrManager)]
        public async Task<IActionResult> EditGet(int id)
        {
            var seedBatch = await _seedBatchRepository.GetById(id);
            if (seedBatch == null) return NotFound();
            ViewBag.Taxonomies = await _taxonomyRepository.GetAll();
            return View(seedBatch);
        }

        [HttpPost]
        [Route("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        [Authorize(Roles = AuthorizationRole.AdminOrManager)]
        public async Task<IActionResult> EditPost(int id)
        {
            var entity = await _seedBatchRepository.GetById(id);
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
                e => e.SeedCount,
                e => e.HarvestDate,
                e => e.ExpectedViabilityMonths,
                e => e.RequiresStratification,
                e => e.EstimatedGerminationRate))
            {
                ViewBag.Taxonomies = await _taxonomyRepository.GetAll();
                return View(entity);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Taxonomies = await _taxonomyRepository.GetAll();
                return View(entity);
            }

            await _seedBatchRepository.Update(entity);
            await CommitAttachmentChanges(id, pendingAttachments, removedAttachmentIds);
            return RedirectToAction("Details", new { id });
        }

        [HttpGet]
        [Route("{id:int}/attachments")]
        [Authorize(Roles = AuthorizationRole.AdminOrManager)]
        public async Task<IActionResult> Attachments(int id)
        {
            var seedBatch = await _seedBatchRepository.GetById(id);
            if (seedBatch == null)
                return NotFound(ErrorMessage.AttachmentSeedBatchNotFound);

            var attachments = await _attachmentRepository.GetActiveForSeedBatch(id);
            return PartialView("_SeedBatchAttachmentList", attachments);
        }

        [HttpPost]
        [Route("{id:int}/attachments/upload")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRole.AdminOrManager)]
        public async Task<IActionResult> UploadAttachment(int id, IFormFile file)
        {
            var seedBatch = await _seedBatchRepository.GetById(id);
            if (seedBatch == null)
                return NotFound(new { error = ErrorMessage.AttachmentSeedBatchNotFound });

            var validationError = ValidateAttachment(file);
            if (validationError != null)
                return BadRequest(new { error = validationError });

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var storedFileName = $"{Guid.NewGuid():N}{extension}";
            var uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "seeds", PendingAttachmentFolder, id.ToString());
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
                filePath = $"/uploads/seeds/{PendingAttachmentFolder}/{id}/{storedFileName}",
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

        private List<PendingAttachment> ReadPendingAttachments(int seedBatchId)
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
                    GetPendingAttachmentPath(seedBatchId, storedFileName)));
            }

            return pendingAttachments;
        }

        private List<int> ReadRemovedAttachmentIds() =>
            Request.Form["RemovedAttachmentIds"]
                .Select(value => int.TryParse(value, out var id) ? id : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();

        private bool PendingAttachmentFilesExist(int seedBatchId, IEnumerable<PendingAttachment> pendingAttachments)
        {
            return pendingAttachments.All(attachment =>
                attachment.PhysicalPath.StartsWith(GetPendingAttachmentDirectory(seedBatchId), StringComparison.OrdinalIgnoreCase) &&
                System.IO.File.Exists(attachment.PhysicalPath));
        }

        private async Task CommitAttachmentChanges(int seedBatchId, List<PendingAttachment> pendingAttachments, List<int> removedAttachmentIds)
        {
            await _attachmentRepository.SoftDeleteManyForSeedBatch(seedBatchId, removedAttachmentIds);

            if (pendingAttachments.Count == 0)
                return;

            var finalDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "seeds", seedBatchId.ToString());
            Directory.CreateDirectory(finalDirectory);

            var attachments = new List<Attachment>();
            foreach (var pendingAttachment in pendingAttachments)
            {
                var finalPath = Path.Combine(finalDirectory, pendingAttachment.StoredFileName);
                System.IO.File.Move(pendingAttachment.PhysicalPath, finalPath, overwrite: false);

                attachments.Add(new Attachment
                {
                    SeedBatchId = seedBatchId,
                    FileName = pendingAttachment.FileName,
                    StoredFileName = pendingAttachment.StoredFileName,
                    FilePath = $"/uploads/seeds/{seedBatchId}/{pendingAttachment.StoredFileName}",
                    ContentType = pendingAttachment.ContentType,
                    FileSize = pendingAttachment.FileSize,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _attachmentRepository.AddRange(attachments);
        }

        private string GetPendingAttachmentDirectory(int seedBatchId) =>
            Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "seeds", PendingAttachmentFolder, seedBatchId.ToString());

        private string GetPendingAttachmentPath(int seedBatchId, string storedFileName) =>
            Path.Combine(GetPendingAttachmentDirectory(seedBatchId), storedFileName);

        private sealed record PendingAttachment(
            string StoredFileName,
            string FileName,
            string ContentType,
            long FileSize,
            string PhysicalPath);
    }
}
