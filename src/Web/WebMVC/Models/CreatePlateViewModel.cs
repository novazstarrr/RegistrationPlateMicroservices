using System.ComponentModel.DataAnnotations;

namespace RTCodingExercise.Microservices.Models
{
    public class CreatePlateViewModel
    {
        [Required(ErrorMessage = "Registration is required")]
        [RegularExpression(@"^[A-Z0-9]{1,7}$", ErrorMessage = "Registration must contain only letters and numbers, max 7 characters")]
        public string? Registration { get; set; }

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "Purchase price must be between £0.01 and £1,000,000")]
        public decimal PurchasePrice { get; set; }

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "Sale price must be between £0.01 and £1,000,000")]
        public decimal SalePrice { get; set; }

        public string? Letters { get; set; }
        public int? Numbers { get; set; }
    }
}