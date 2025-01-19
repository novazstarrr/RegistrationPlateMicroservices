using System;
using System.Collections.Generic;

namespace Catalog.API.DTOs.Common
{
    public class PagedListDto<T>
    {
        public IReadOnlyList<T> Items { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }
        public string? SortOrder { get; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PagedListDto(
            IReadOnlyList<T> items,
            int totalCount,
            int pageNumber,
            int pageSize,
            string? sortOrder = null)
        {
            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            SortOrder = sortOrder;
        }
    }
}