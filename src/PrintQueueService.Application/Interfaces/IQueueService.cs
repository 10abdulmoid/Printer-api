using PrintQueueService.Application.DTOs.Queues;

namespace PrintQueueService.Application.Interfaces;

public interface IQueueService
{
    Task<QueueResponse> CreateAsync(CreateQueueRequest request);
    Task<IEnumerable<QueueResponse>> GetAllAsync();
    Task<QueueResponse?> GetByIdAsync(Guid id);
    Task<QueueResponse?> UpdatePauseStatusAsync(Guid id, UpdateQueuePauseRequest request);
}
