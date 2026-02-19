namespace PrintQueueService.Domain.Enums;

public enum PrintJobStatus
{
    Queued = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Canceled = 4
}
