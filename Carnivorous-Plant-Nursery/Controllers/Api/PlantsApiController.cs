using Carnivorous_Plant_Nursery.Models;
using Carnivorous_Plant_Nursery.Models.Api;
using Carnivorous_Plant_Nursery.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Carnivorous_Plant_Nursery.Controllers.Api
{
    [ApiController]
    [Route("api/plants")]
    public class PlantsApiController : ControllerBase
    {
        private readonly PlantRepository _plantRepository;

        public PlantsApiController(PlantRepository plantRepository)
        {
            _plantRepository = plantRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlantDto>>> GetAll(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? webshopOnly,
            [FromQuery] PlantStage? stage,
            [FromQuery] HealthState? healthStatus)
        {
            var plants = await _plantRepository.Search(searchTerm, webshopOnly, stage, healthStatus);
            return Ok(plants.Select(p => p.ToDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PlantDto>> GetById(int id)
        {
            var plant = await _plantRepository.GetById(id);
            return plant == null ? NotFound() : Ok(plant.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<PlantDto>> Create([FromBody] PlantWriteDto dto)
        {
            var plant = new Plant();
            dto.ApplyTo(plant);

            try
            {
                await _plantRepository.Add(plant);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { error = ErrorMessage.ApiInvalidReference });
            }

            var saved = await _plantRepository.GetById(plant.Id);
            return CreatedAtAction(nameof(GetById), new { id = plant.Id }, saved?.ToDto() ?? plant.ToDto());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<PlantDto>> Update(int id, [FromBody] PlantWriteDto dto)
        {
            var plant = await _plantRepository.GetById(id);
            if (plant == null) return NotFound();

            dto.ApplyTo(plant);

            try
            {
                await _plantRepository.Update(plant);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { error = ErrorMessage.ApiInvalidReference });
            }

            var saved = await _plantRepository.GetById(id);
            return Ok(saved?.ToDto() ?? plant.ToDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var plant = await _plantRepository.GetById(id);
            if (plant == null) return NotFound();

            try
            {
                await _plantRepository.Delete(id);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }

            return NoContent();
        }
    }
}
