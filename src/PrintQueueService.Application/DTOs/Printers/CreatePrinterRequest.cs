using System.ComponentModel.DataAnnotations;

namespace PrintQueueService.Application.DTOs.Printers;

public class CreatePrinterRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
    public string? Location { get; set; }

    public string? Status { get; set; }

    [StringLength(500, ErrorMessage = "Capabilities cannot exceed 500 characters")]
    public string? Capabilities { get; set; }
}
