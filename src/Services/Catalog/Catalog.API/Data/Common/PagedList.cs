using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Data.Common
{
    public class PagedList<T>
    {
        public IReadOnlyList<T> Items { get; }
        public int PageNumber { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }
        public string? SortOrder { get; }

        public PagedList(
            IReadOnlyList<T> items,
            int totalCount,
            int pageNumber,
            int pageSize,
            string? sortOrder = null)
        {
            Items = items;
            PageNumber = pageNumber;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            SortOrder = sortOrder;
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        public static PagedList<T> Create(IEnumerable<T> items, int pageNumber, int pageSize, int totalCount)
        {
            return new PagedList<T>(items.ToList(), totalCount, pageNumber, pageSize);
        }
    }
}