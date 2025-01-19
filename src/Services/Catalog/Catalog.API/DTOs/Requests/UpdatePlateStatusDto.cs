using System.ComponentModel.DataAnnotations;

namespace Catalog.API.DTOs.Requests
{
    public class UpdatePlateStatusDto
    {
        [Required]
        public int Status { get; set; }
    }
}