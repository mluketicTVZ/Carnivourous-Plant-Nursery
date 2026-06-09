using Carnivorous_Plant_Nursery.Models;
using Carnivorous_Plant_Nursery.Models.Api;
using Carnivorous_Plant_Nursery.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Carnivorous_Plant_Nursery.Controllers.Api
{
    [ApiController]
    [Route("api/care-profiles")]
    public class CareProfilesApiController : ControllerBase
    {
        private readonly CareProfileRepository _careProfileRepository;

        public CareProfilesApiController(CareProfileRepository careProfileRepository)
        {
            _careProfileRepository = careProfileRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CareProfileDto>>> GetAll([FromQuery] string? searchTerm, [FromQuery] LightLevel? requiredLight)
        {
            var careProfiles = await _careProfileRepository.Search(searchTerm, requiredLight);
            return Ok(careProfiles.Select(c => c.ToDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CareProfileDto>> GetById(int id)
        {
            var careProfile = await _careProfileRepository.GetById(id);
            return careProfile == null ? NotFound() : Ok(careProfile.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<CareProfileDto>> Create([FromBody] CareProfileWriteDto dto)
        {
            var careProfile = new CareProfile();
            dto.ApplyTo(careProfile);

            try
            {
                await _careProfileRepository.Add(careProfile);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { error = ex.Message });
            }

            return CreatedAtAction(nameof(GetById), new { id = careProfile.Id }, careProfile.ToDto());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CareProfileDto>> Update(int id, [FromBody] CareProfileWriteDto dto)
        {
            var careProfile = await _careProfileRepository.GetById(id);
            if (careProfile == null) return NotFound();

            dto.ApplyTo(careProfile);

            try
            {
                await _careProfileRepository.Update(careProfile);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { error = ex.Message });
            }

            return Ok(careProfile.ToDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var careProfile = await _careProfileRepository.GetById(id);
            if (careProfile == null) return NotFound();

            try
            {
                await _careProfileRepository.Delete(id);
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
