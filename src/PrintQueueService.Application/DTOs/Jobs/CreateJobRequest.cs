using System.ComponentModel.DataAnnotations;

namespace PrintQueueService.Application.DTOs.Jobs;

public class CreateJobRequest
{
    [Required(ErrorMessage = "QueueId is required")]
    public Guid QueueId { get; set; }

    [Required(ErrorMessage = "DocumentName is required")]
    [StringLength(200, ErrorMessage = "DocumentName cannot exceed 200 characters")]
    public string DocumentName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Pages is required")]
    [Range(1, 500, ErrorMessage = "Pages must be between 1 and 500")]
    public int Pages { get; set; }

    [StringLength(100, ErrorMessage = "SubmittedBy cannot exceed 100 characters")]
    public string? SubmittedBy { get; set; }
}
