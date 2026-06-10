using Carnivorous_Plant_Nursery.Models.Api;
using Carnivorous_Plant_Nursery.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Carnivorous_Plant_Nursery.Controllers.Api
{
    [ApiController]
    [Route("api/inventory")]
    public class InventoryApiController : ControllerBase
    {
        private readonly InventoryRepository _inventoryRepository;

        public InventoryApiController(InventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemSummaryDto>>> GetAll([FromQuery] string? searchTerm, [FromQuery] bool? webshopOnly)
        {
            var items = await _inventoryRepository.SearchAsync(searchTerm, webshopOnly);
            return Ok(items.Select(i => i.ToSummaryDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<InventoryItemSummaryDto>> GetById(int id)
        {
            var item = await _inventoryRepository.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item.ToSummaryDto());
        }
    }
}
