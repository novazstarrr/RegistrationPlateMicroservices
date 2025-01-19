using System;
using System.Threading.Tasks;
using Catalog.API.DTOs;
using Catalog.Domain;
using Catalog.API.DTOs.Common;

namespace Catalog.API.Services
{
    public interface IPlateService
    {
        Task<PlateDto> CreatePlateAsync(CreatePlateDto createDto);
        Task<PlateDto> GetByIdAsync(Guid id);
        Task<PagedListDto<PlateDto>> GetPlatesAsync(
            int pageNumber = 1,
            int pageSize = 20,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? sortOrder = null,
            string? nameMatch = null);
        Task<PlateDto> UpdateStatusAsync(Guid id, PlateStatus newStatus);
    }
}