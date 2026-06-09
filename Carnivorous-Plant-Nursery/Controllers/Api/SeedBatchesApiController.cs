using Carnivorous_Plant_Nursery.Models;
using Carnivorous_Plant_Nursery.Models.Api;
using Carnivorous_Plant_Nursery.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Carnivorous_Plant_Nursery.Controllers.Api
{
    [ApiController]
    [Route("api/seeds")]
    public class SeedBatchesApiController : ControllerBase
    {
        private readonly SeedBatchRepository _seedBatchRepository;

        public SeedBatchesApiController(SeedBatchRepository seedBatchRepository)
        {
            _seedBatchRepository = seedBatchRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SeedBatchDto>>> GetAll([FromQuery] string? searchTerm, [FromQuery] bool? availableInWebshop)
        {
            var seedBatches = await _seedBatchRepository.Search(searchTerm, availableInWebshop);
            return Ok(seedBatches.Select(s => s.ToDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SeedBatchDto>> GetById(int id)
        {
            var seedBatch = await _seedBatchRepository.GetById(id);
            return seedBatch == null ? NotFound() : Ok(seedBatch.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<SeedBatchDto>> Create([FromBody] SeedBatchWriteDto dto)
        {
            var seedBatch = new SeedBatch();
            dto.ApplyTo(seedBatch);

            try
            {
                await _seedBatchRepository.Add(seedBatch);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { error = ErrorMessage.ApiInvalidReference });
            }

            var saved = await _seedBatchRepository.GetById(seedBatch.Id);
            return CreatedAtAction(nameof(GetById), new { id = seedBatch.Id }, saved?.ToDto() ?? seedBatch.ToDto());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<SeedBatchDto>> Update(int id, [FromBody] SeedBatchWriteDto dto)
        {
            var seedBatch = await _seedBatchRepository.GetById(id);
            if (seedBatch == null) return NotFound();

            dto.ApplyTo(seedBatch);

            try
            {
                await _seedBatchRepository.Update(seedBatch);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { error = ErrorMessage.ApiInvalidReference });
            }

            var saved = await _seedBatchRepository.GetById(id);
            return Ok(saved?.ToDto() ?? seedBatch.ToDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var seedBatch = await _seedBatchRepository.GetById(id);
            if (seedBatch == null) return NotFound();

            try
            {
                await _seedBatchRepository.Delete(id);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }

            return NoContent();
        }
    }
}
