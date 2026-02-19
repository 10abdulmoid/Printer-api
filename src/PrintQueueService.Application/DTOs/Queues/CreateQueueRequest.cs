using System.ComponentModel.DataAnnotations;

namespace PrintQueueService.Application.DTOs.Queues;

public class CreateQueueRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "PrinterId is required")]
    public Guid PrinterId { get; set; }
}
