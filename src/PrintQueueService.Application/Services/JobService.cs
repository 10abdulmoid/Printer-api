using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PrintQueueService.Application.Data;
using PrintQueueService.Application.DTOs.Jobs;
using PrintQueueService.Application.Interfaces;
using PrintQueueService.Domain.Entities;
using PrintQueueService.Domain.Enums;

namespace PrintQueueService.Application.Services;

public class JobService : IJobService
{
    private readonly AppDbContext _context;
    private readonly ILogger<JobService> _logger;

    public JobService(AppDbContext context, ILogger<JobService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<JobResponse> CreateAsync(CreateJobRequest request)
    {
        // Verify queue exists
        var queueExists = await _context.Queues.AnyAsync(q => q.Id == request.QueueId);
        if (!queueExists)
        {
            throw new KeyNotFoundException($"Queue with ID {request.QueueId} not found");
        }

        var job = new PrintJob
        {
            Id = Guid.NewGuid(),
            QueueId = request.QueueId,
            DocumentName = request.DocumentName,
            Pages = request.Pages,
            SubmittedBy = request.SubmittedBy,
            Status = PrintJobStatus.Queued,
            Attempts = 0
        };

        _context.PrintJobs.Add(job);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created job {JobId} for document {DocumentName} in queue {QueueId}", 
            job.Id, job.DocumentName, job.QueueId);

        return MapToResponse(job);
    }

    public async Task<JobResponse?> GetByIdAsync(Guid id)
    {
        var job = await _context.PrintJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id);

        return job == null ? null : MapToResponse(job);
    }

    public async Task<PaginatedJobsResponse> GetAllAsync(int page, int pageSize, string? status = null, Guid? queueId = null)
    {
        var query = _context.PrintJobs.AsNoTracking();

        // Apply filters
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<PrintJobStatus>(status, true, out var statusEnum))
        {
            query = query.Where(j => j.Status == statusEnum);
        }

        if (queueId.HasValue)
        {
            query = query.Where(j => j.QueueId == queueId.Value);
        }

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination
        var jobs = await query
            .OrderByDescending(j => j.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedJobsResponse
        {
            Items = jobs.Select(MapToResponse).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<JobResponse?> CancelAsync(Guid id)
    {
        var job = await _context.PrintJobs.FindAsync(id);
        if (job == null)
        {
            return null;
        }

        // Can only cancel Queued or Processing jobs
        if (job.Status == PrintJobStatus.Completed || 
            job.Status == PrintJobStatus.Failed || 
            job.Status == PrintJobStatus.Canceled)
        {
            throw new InvalidOperationException($"Cannot cancel job with status {job.Status}");
        }

        job.Status = PrintJobStatus.Canceled;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Canceled job {JobId}", id);

        return MapToResponse(job);
    }

    private static JobResponse MapToResponse(PrintJob job) => new()
    {
        Id = job.Id,
        QueueId = job.QueueId,
        DocumentName = job.DocumentName,
        Pages = job.Pages,
        SubmittedBy = job.SubmittedBy,
        Status = job.Status.ToString(),
        Attempts = job.Attempts,
        ErrorMessage = job.ErrorMessage,
        CreatedAtUtc = job.CreatedAtUtc,
        UpdatedAtUtc = job.UpdatedAtUtc
    };
}
