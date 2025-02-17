using Catalog.Domain;

namespace Catalog.API.DTOs.Responses
{
    public class PlateDto
    {
        public Guid Id { get; set; }
        public string? Registration { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public string? Letters { get; set; }
        public int Numbers { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
        public int Status { get; set; }
    }
}