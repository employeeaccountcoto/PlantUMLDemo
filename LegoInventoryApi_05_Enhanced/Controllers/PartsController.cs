using LegoInventoryApi.Models;
using LegoInventoryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LegoInventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartsController(PartService partService, ILogger<PartsController> logger) : ControllerBase
    {
        private readonly PartService _partService = partService;
        private readonly ILogger<PartsController> _logger = logger;

        /// <summary>
        /// Gets all parts
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Part>>> GetParts()
        {
            var parts = await _partService.GetAllPartsAsync();
            return Ok(parts);
        }

        /// <summary>
        /// Gets a part by its part number
        /// </summary>
        [HttpGet("{partNum}")]
        public async Task<ActionResult<Part>> GetPart(string partNum)
        {
            var part = await _partService.GetPartByPartNumAsync(partNum);

            if (part == null)
            {
                _logger.LogInformation("Part with number {PartNum} not found", partNum);
                return NotFound();
            }

            return Ok(part);
        }
    }
}
