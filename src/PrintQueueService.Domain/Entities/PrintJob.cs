using PrintQueueService.Domain.Common;
using PrintQueueService.Domain.Enums;

namespace PrintQueueService.Domain.Entities;

public class PrintJob : BaseEntity
{
    public Guid QueueId { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public int Pages { get; set; }
    public string? SubmittedBy { get; set; }
    public PrintJobStatus Status { get; set; } = PrintJobStatus.Queued;
    public int Attempts { get; set; }
    public string? ErrorMessage { get; set; }

    // Navigation property
    public Queue Queue { get; set; } = null!;
}
