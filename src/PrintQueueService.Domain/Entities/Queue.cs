using PrintQueueService.Domain.Common;

namespace PrintQueueService.Domain.Entities;

public class Queue : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid PrinterId { get; set; }
    public bool IsPaused { get; set; }

    // Navigation properties
    public Printer Printer { get; set; } = null!;
    public ICollection<PrintJob> PrintJobs { get; set; } = new List<PrintJob>();
}
