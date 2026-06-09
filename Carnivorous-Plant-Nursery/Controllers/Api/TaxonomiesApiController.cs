using Carnivorous_Plant_Nursery.Models;
using Carnivorous_Plant_Nursery.Models.Api;
using Carnivorous_Plant_Nursery.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Carnivorous_Plant_Nursery.Controllers.Api
{
    [ApiController]
    [Route("api/taxonomies")]
    public class TaxonomiesApiController : ControllerBase
    {
        private readonly TaxonomyRepository _taxonomyRepository;

        public TaxonomiesApiController(TaxonomyRepository taxonomyRepository)
        {
            _taxonomyRepository = taxonomyRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaxonomyDto>>> GetAll([FromQuery] string? searchTerm)
        {
            var taxonomies = await _taxonomyRepository.Search(searchTerm ?? string.Empty);
            return Ok(taxonomies.Select(t => t.ToDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaxonomyDto>> GetById(int id)
        {
            var taxonomy = await _taxonomyRepository.GetById(id);
            return taxonomy == null ? NotFound() : Ok(taxonomy.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<TaxonomyDto>> Create([FromBody] TaxonomyWriteDto dto)
        {
            var taxonomy = new Taxonomy();
            dto.ApplyTo(taxonomy);

            try
            {
                await _taxonomyRepository.Add(taxonomy);
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { error = ErrorMessage.ApiInvalidReference });
            }

            var saved = await _taxonomyRepository.GetById(taxonomy.Id);
            return CreatedAtAction(nameof(GetById), new { id = taxonomy.Id }, saved?.ToDto() ?? taxonomy.ToDto());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TaxonomyDto>> Update(int id, [FromBody] TaxonomyWriteDto dto)
        {
            var taxonomy = await _taxonomyRepository.GetById(id);
            if (taxonomy == null) return NotFound();

            dto.ApplyTo(taxonomy);

            try
            {
                await _taxonomyRepository.Update(taxonomy);
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { error = ErrorMessage.ApiInvalidReference });
            }

            var saved = await _taxonomyRepository.GetById(id);
            return Ok(saved?.ToDto() ?? taxonomy.ToDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var taxonomy = await _taxonomyRepository.GetById(id);
            if (taxonomy == null) return NotFound();

            try
            {
                await _taxonomyRepository.Delete(id);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { error = ErrorMessage.ApiInvalidReference });
            }

            return NoContent();
        }
    }
}
