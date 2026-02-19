namespace PrintQueueService.Application.DTOs.Jobs;

public class JobResponse
{
    public Guid Id { get; set; }
    public Guid QueueId { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public int Pages { get; set; }
    public string? SubmittedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Attempts { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
