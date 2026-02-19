using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PrintQueueService.Application.Data;
using PrintQueueService.Domain.Enums;

namespace PrintQueueService.Application.Services;

public class PrintJobProcessorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PrintJobProcessorService> _logger;
    private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(3);
    private const int MaxJobsPerBatch = 5;

    public PrintJobProcessorService(IServiceProvider serviceProvider, ILogger<PrintJobProcessorService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Print Job Processor Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessJobsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing print jobs");
            }

            await Task.Delay(_processingInterval, stoppingToken);
        }

        _logger.LogInformation("Print Job Processor Service stopped");
    }

    private async Task ProcessJobsAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Find queued jobs where queue is not paused and printer is online
        var jobsToProcess = await context.PrintJobs
            .Include(j => j.Queue)
                .ThenInclude(q => q.Printer)
            .Where(j => j.Status == PrintJobStatus.Queued)
            .Where(j => !j.Queue.IsPaused)
            .Where(j => j.Queue.Printer.Status == PrinterStatus.Online)
            .OrderBy(j => j.CreatedAtUtc)
            .Take(MaxJobsPerBatch)
            .ToListAsync(stoppingToken);

        foreach (var job in jobsToProcess)
        {
            if (stoppingToken.IsCancellationRequested)
                break;

            // Immediately mark as Processing to prevent double processing
            job.Status = PrintJobStatus.Processing;
            job.Attempts++;
            await context.SaveChangesAsync(stoppingToken);

            _logger.LogInformation("Processing job {JobId} - Document: {DocumentName}, Pages: {Pages}", 
                job.Id, job.DocumentName, job.Pages);

            // Simulate processing work
            await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);

            // Failure simulation: if pages > 300, fail the job
            if (job.Pages > 300)
            {
                job.Status = PrintJobStatus.Failed;
                job.ErrorMessage = "Job too large for demo processor (max 300 pages)";
                _logger.LogWarning("Job {JobId} failed: {ErrorMessage}", job.Id, job.ErrorMessage);
            }
            else
            {
                job.Status = PrintJobStatus.Completed;
                _logger.LogInformation("Job {JobId} completed successfully", job.Id);
            }

            await context.SaveChangesAsync(stoppingToken);
        }
    }
}
