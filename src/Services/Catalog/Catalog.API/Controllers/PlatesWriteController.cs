using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Catalog.API.DTOs.Requests;
using Catalog.API.Exceptions;
using Catalog.API.Services;
using Catalog.Domain;
using System.Threading.Tasks;
using Catalog.API.DTOs.Responses;
using MassTransit;
using IntegrationEvents;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("plates")]
    public class PlatesWriteController : ControllerBase
    {
        private readonly IPlateService _plateService;
        private readonly ILogger<PlatesWriteController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public PlatesWriteController(
            IPlateService plateService,
            ILogger<PlatesWriteController> logger,
            IPublishEndpoint publishEndpoint)
        {
            _plateService = plateService;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<PlateDto>> CreatePlate([FromBody] CreatePlateDto createDto)
        {
            _logger.LogInformation("Received create plate request: {@CreateDto}", createDto);
            var plate = await _plateService.CreatePlateAsync(createDto);
            return CreatedAtAction(nameof(PlatesReadController.GetPlate),
                "PlatesRead",
                new { id = plate.Id },
                plate);
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<PlateDto>> UpdateStatus(Guid id, [FromBody] UpdatePlateStatusDto updateDto)
        {
            _logger.LogInformation("Attempting to update status for plate {PlateId} to {Status}", id, updateDto.Status);

            var plateStatus = (PlateStatus)updateDto.Status;
            var plate = await _plateService.UpdateStatusAsync(id, plateStatus);
            if (plate == null)
                return NotFound();


            if (plateStatus == PlateStatus.Reserved)
            {
                await _publishEndpoint.Publish(new PlateReservationEvent(
                    plateId: plate.Id,
                    registration: plate.Registration,
                    isReserved: true
                ));
            }

            return Ok(plate);
        }
    }
}
