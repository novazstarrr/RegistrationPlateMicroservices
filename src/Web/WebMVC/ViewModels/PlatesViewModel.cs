namespace WebMVC.ViewModels
{
    public class PlatesViewModel
    {
        public IReadOnlyList<PlateViewModel> Items { get; set; } = new List<PlateViewModel>();
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortOrder { get; set; }
        public string? NameMatch { get; set; }
    }

    public class PlateViewModel
    {
        public Guid Id { get; set; }
        public string? Registration { get; set; }
        public decimal SalePrice { get; set; }
        public string? Letters { get; set; }
        public int Numbers { get; set; }
        public bool IsReserved { get; set; }
    }
} 