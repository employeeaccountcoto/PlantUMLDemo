using LegoInventoryApi.Models;
using LegoInventoryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LegoInventoryApi.Controllers
{    [ApiController]
    [Route("api/[controller]")]
    public class PartsController(PartService partService, ILogger<PartsController> logger) : ControllerBase
    {
        private readonly PartService _partService = partService;
        private readonly ILogger<PartsController> _logger = logger;        /// <summary>
        /// 🔍 Gets all LEGO parts in your inventory
        /// </summary>
        /// <returns>A collection of all your awesome LEGO parts</returns>
        /// <response code="200">Successfully retrieved all parts - ready to build!</response>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get All Parts", 
            Description = "Retrieve all LEGO parts in your inventory. Perfect for browsing your collection!"
        )]
        [SwaggerResponse(200, "Success! Here are all your LEGO parts", typeof(IEnumerable<Part>))]
        [SwaggerResponse(500, "Oops! Something went wrong on our end")]
        public async Task<ActionResult<IEnumerable<Part>>> GetParts()
        {
            var parts = await _partService.GetAllPartsAsync();
            return Ok(parts);
        }        /// <summary>
        /// 🎯 Find a specific LEGO part by its part number
        /// </summary>
        /// <param name="partNum">The unique part number to search for</param>
        /// <returns>The LEGO part details if found</returns>
        /// <response code="200">Found it! Here's your LEGO part</response>
        /// <response code="404">Sorry, that part number wasn't found in your inventory</response>
        [HttpGet("{partNum}")]
        [SwaggerOperation(
            Summary = "Get Part by Number", 
            Description = "Find a specific LEGO part using its unique part number. Great for locating that exact piece you need!"
        )]
        [SwaggerResponse(200, "Found it! Here's your LEGO part", typeof(Part))]
        [SwaggerResponse(404, "Part not found - maybe it's lost in the carpet?")]
        [SwaggerResponse(500, "Oops! Something went wrong on our end")]
        public async Task<ActionResult<Part>> GetPart(
            [SwaggerParameter("The unique LEGO part number (e.g., '3001')", Required = true)] 
            string partNum)
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
