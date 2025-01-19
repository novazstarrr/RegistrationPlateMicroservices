using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Catalog.API.Data.Common;
using Catalog.API.Data.Interface;
using Catalog.Domain;
using Catalog.Domain.Helpers;
using AutoMapper;
using Catalog.API.DTOs;
using Catalog.API.DTOs.Common;
using Catalog.API.DTOs.Responses;
using MassTransit;
using IntegrationEvents;
using System.Linq;

namespace Catalog.API.Services
{
    public class PlateService : IPlateService
    {
        private readonly IPlateRepository _repository;
        private readonly ILogger<PlateService> _logger;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public PlateService(
            IPlateRepository repository,
            ILogger<PlateService> logger,
            IMapper mapper,
            IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<PlateDto> CreatePlateAsync(CreatePlateDto createDto)
        {
            _logger.LogInformation("Creating new plate with registration: {Registration}", createDto.Registration);

            var plate = _mapper.Map<Plate>(createDto);
            plate.Id = Guid.NewGuid();
            plate.Registration = plate.Registration?.ToUpperInvariant() ?? string.Empty;

            plate.SalePrice = PriceCalculator.EnsureMinimumSalePrice(
                plate.PurchasePrice,
                plate.SalePrice);

            var created = await _repository.AddAsync(plate);

            if (created == null)
            {
                _logger.LogWarning("Failed to create plate. Registration might be duplicate: {Registration}", plate.Registration);
                throw new PlateValidationException(new Dictionary<string, string[]>
                {
                    { "Registration", new[] { "A plate with this registration already exists." } }
                });
            }

            _logger.LogInformation("Successfully created plate with ID: {PlateId}", created.Id);
            return _mapper.Map<PlateDto>(created);
        }

        public async Task<PlateDto> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting plate by ID: {PlateId}", id);

            var plate = await _repository.GetByIdAsync(id);

            if (plate == null)
            {
                _logger.LogWarning("Plate with ID {PlateId} not found", id);
                throw new PlateNotFoundException(id);
            }

            return _mapper.Map<PlateDto>(plate);
        }

        public async Task<PagedListDto<PlateDto>> GetPlatesAsync(
            int pageNumber = 1,
            int pageSize = 20,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? sortOrder = null,
            string? nameMatch = null)
        {
            try
            {
                _logger.LogInformation(
                    "Getting plates with filters: Page={PageNumber}, Size={PageSize}, MinPrice={MinPrice}, MaxPrice={MaxPrice}, SortOrder={SortOrder}, NameMatch={NameMatch}",
                    pageNumber, pageSize, minPrice, maxPrice, sortOrder, nameMatch);


                var plates = await _repository.GetPlatesAsync(
                    pageNumber,
                    pageSize,
                    minPrice,
                    maxPrice,
                    sortOrder,
                    nameMatch);


                if (!string.IsNullOrWhiteSpace(nameMatch))
                {
                    var normalizedSearch = nameMatch.ToUpperInvariant().Replace(" ", "");
                    var exactMatch = plates.Items.FirstOrDefault(p =>
                        p.Registration?.ToUpperInvariant().Replace(" ", "") == normalizedSearch);

                    if (exactMatch != null)
                    {
                        // Exact match found; return only that plate
                        return new PagedListDto<PlateDto>(
                            items: new List<PlateDto> { _mapper.Map<PlateDto>(exactMatch) },
                            totalCount: 1,
                            pageNumber: 1,
                            pageSize: pageSize,
                            sortOrder: plates.SortOrder);
                    }
                    else
                    {
                        var filteredItems = plates.Items
                            .Where(x => x.Status == PlateStatus.ForSale)
                            .ToList();

                        plates = new PagedList<Plate>(
                            items: filteredItems,
                            totalCount: filteredItems.Count,
                            pageNumber: plates.PageNumber,
                            pageSize: pageSize,
                            sortOrder: plates.SortOrder
                        );
                    }
                }

                var plateDtos = _mapper.Map<List<PlateDto>>(plates.Items);

                return new PagedListDto<PlateDto>(
                    items: plateDtos,
                    totalCount: plates.TotalCount,
                    pageNumber: plates.PageNumber,
                    pageSize: pageSize,
                    sortOrder: plates.SortOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving plates for page {PageNumber}", pageNumber);
                throw;
            }
        }


        public async Task<PlateDto> UpdateStatusAsync(Guid id, PlateStatus newStatus)
        {
            _logger.LogInformation("Attempting to update status for plate {PlateId} to {Status}", id, newStatus);

            var plate = await _repository.UpdateStatusAsync(id, newStatus);

            if (plate == null)
            {
                _logger.LogWarning("Plate with ID {PlateId} not found when trying to update status", id);
                throw new PlateNotFoundException(id);
            }



            _logger.LogInformation("Successfully updated status for plate {PlateId} to {Status}",
                id, plate.Status);

            return _mapper.Map<PlateDto>(plate);
        }
    }
}