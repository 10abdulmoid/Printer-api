namespace PrintQueueService.Application.DTOs.Queues;

public class QueueResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid PrinterId { get; set; }
    public bool IsPaused { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
