namespace Catalog.API.DTOs.Requests
{
    public class CreatePlateDto
    {
        public string? Registration { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public string? Letters { get; set; }
        public int Numbers { get; set; }
    }
}