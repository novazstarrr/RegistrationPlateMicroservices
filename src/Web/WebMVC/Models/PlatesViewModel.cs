namespace RTCodingExercise.Microservices.Models
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


        public bool IsSortedAscending => SortOrder?.ToLower() == "asc";
        public bool IsSortedDescending => SortOrder?.ToLower() == "desc";
    }

    public class PlateViewModel
    {
        public Guid Id { get; set; }
        public string? Registration { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
        public int Status { get; set; }
    }
}