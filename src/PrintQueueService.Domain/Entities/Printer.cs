using PrintQueueService.Domain.Common;
using PrintQueueService.Domain.Enums;

namespace PrintQueueService.Domain.Entities;

public class Printer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public PrinterStatus Status { get; set; } = PrinterStatus.Online;
    public string? Capabilities { get; set; }

    // Navigation property
    public ICollection<Queue> Queues { get; set; } = new List<Queue>();
}
