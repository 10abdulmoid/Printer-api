using PrintQueueService.Application.DTOs.Jobs;

namespace PrintQueueService.Application.Interfaces;

public interface IJobService
{
    Task<JobResponse> CreateAsync(CreateJobRequest request);
    Task<JobResponse?> GetByIdAsync(Guid id);
    Task<PaginatedJobsResponse> GetAllAsync(int page, int pageSize, string? status = null, Guid? queueId = null);
    Task<JobResponse?> CancelAsync(Guid id);
}
