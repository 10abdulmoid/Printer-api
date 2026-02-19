using System.ComponentModel.DataAnnotations;

namespace PrintQueueService.Application.DTOs.Printers;

public class UpdatePrinterStatusRequest
{
    [Required(ErrorMessage = "Status is required")]
    [RegularExpression("^(Online|Offline)$", ErrorMessage = "Status must be 'Online' or 'Offline'")]
    public string Status { get; set; } = string.Empty;
}
