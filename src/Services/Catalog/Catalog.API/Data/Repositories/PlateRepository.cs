using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Catalog.API.Data.Common;
using Catalog.API.Data.Interface;
using Catalog.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using System.Text.RegularExpressions;
using Catalog.Domain.Helpers;

namespace Catalog.API.Data.Repositories
{
    public class PlateRepository : IPlateRepository
    {
        private readonly ApplicationDbContext _context;

        public PlateRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<PagedList<Plate>> GetPlatesAsync(
            int pageNumber = 1,
            int pageSize = 20,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? sortOrder = null,
            string? nameMatch = null)
        {
            var query = _context.Plates.AsNoTracking();

            if (minPrice.HasValue && minPrice.Value >= 0)
            {
                query = query.Where(p => p.SalePrice >= minPrice.Value);
            }

            if (maxPrice.HasValue && maxPrice.Value >= 0)
            {
                query = query.Where(p => p.SalePrice <= maxPrice.Value);
            }

            if (!string.IsNullOrWhiteSpace(nameMatch))
            {
                var likePattern = PlateMatchingHelper.BuildSqlLikePattern(nameMatch);
                query = query.Where(p => EF.Functions.Like(p.Registration!, likePattern));
            }

            query = sortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.SalePrice)
                : query.OrderBy(p => p.SalePrice);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            pageNumber = Math.Min(Math.Max(1, pageNumber), Math.Max(1, totalPages));

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<Plate>(
                items: items,
                totalCount: totalCount,
                pageNumber: pageNumber,
                pageSize: pageSize,
                sortOrder: sortOrder ?? "asc");
        }

        public async Task<Plate?> GetByIdAsync(Guid id)
        {
            return await _context.Plates
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Plate?> AddAsync(Plate plate)
        {
            if (plate == null) return null;

            var exists = await _context.Plates
                .AnyAsync(p => p.Registration == plate.Registration);
            if (exists) return null;

            await _context.Plates.AddAsync(plate);
            await _context.SaveChangesAsync();
            return plate;
        }

        public async Task<Plate?> UpdateStatusAsync(Guid id, PlateStatus newStatus)
        {
            var plate = await _context.Plates.FindAsync(id);
            if (plate == null) return null;

            plate.Status = newStatus;
            await _context.SaveChangesAsync();
            return plate;
        }
    }
}
