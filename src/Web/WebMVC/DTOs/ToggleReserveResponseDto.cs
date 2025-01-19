namespace WebMVC.DTOs
{
    public class ToggleReserveResponseDto
    {
        public bool IsReserved { get; set; }
        public string Registration { get; set; } = string.Empty;
    }
} 