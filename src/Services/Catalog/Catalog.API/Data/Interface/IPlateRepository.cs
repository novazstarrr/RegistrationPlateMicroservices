using Catalog.API.Data.Common;
using Catalog.Domain;

namespace Catalog.API.Data.Interface
{
    public interface IPlateRepository
    {
        Task<PagedList<Plate>> GetPlatesAsync(
            int pageNumber = 1,
            int pageSize = 20,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? sortOrder = null,
            string? nameMatch = null);
        Task<Plate?> GetByIdAsync(Guid id);
        Task<Plate?> AddAsync(Plate plate);
        Task<Plate?> UpdateStatusAsync(Guid id, PlateStatus newStatus);
    }
}