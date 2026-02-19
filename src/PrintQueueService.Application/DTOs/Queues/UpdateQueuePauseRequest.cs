using System.ComponentModel.DataAnnotations;

namespace PrintQueueService.Application.DTOs.Queues;

public class UpdateQueuePauseRequest
{
    [Required(ErrorMessage = "IsPaused is required")]
    public bool IsPaused { get; set; }
}
