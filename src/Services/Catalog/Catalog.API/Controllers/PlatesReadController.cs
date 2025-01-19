using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Catalog.API.Services;
using Catalog.API.Data.Common;
using Catalog.Domain;
using Catalog.API.DTOs.Common;
using Catalog.API.DTOs.Responses;
using System;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("plates")]
    public class PlatesReadController : ControllerBase
    {
        private readonly IPlateService _plateService;
        private readonly ILogger<PlatesReadController> _logger;
        private readonly IMapper _mapper;

        public PlatesReadController(
            IPlateService plateService,
            ILogger<PlatesReadController> logger,
            IMapper mapper)
        {
            _plateService = plateService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponseDto<PlateDto>>> GetPlates(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? sortOrder = "asc",
            [FromQuery] string? nameMatch = null)
        {
            _logger.LogInformation(
                "Getting plates. Page: {PageNumber}, Size: {PageSize}, Sort: {SortOrder}",
                pageNumber, pageSize, sortOrder);

            try
            {
                var result = await _plateService.GetPlatesAsync(
                    pageNumber,
                    pageSize,
                    minPrice,
                    maxPrice,
                    sortOrder ?? "asc",
                    nameMatch);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting plates");
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlateDto>> GetPlate(Guid id)
        {
            var plate = await _plateService.GetByIdAsync(id);
            if (plate == null)
                return NotFound();

            return Ok(plate);
        }
    }
}
