using Microsoft.Extensions.Logging;
using Moq;
using PrintQueueService.Application.DTOs.Jobs;
using PrintQueueService.Application.DTOs.Printers;
using PrintQueueService.Application.DTOs.Queues;
using PrintQueueService.Application.Services;
using PrintQueueService.Domain.Enums;
using PrintQueueService.Tests.TestHelpers;
using Xunit;

namespace PrintQueueService.Tests.Services;

public class JobServiceTests
{
    private async Task<(PrinterService printerService, QueueService queueService, JobService jobService, Guid queueId)> 
        SetupServicesWithQueue()
    {
        var context = TestDbContextFactory.CreateInMemoryContext();
        
        var printerLogger = Mock.Of<ILogger<PrinterService>>();
        var queueLogger = Mock.Of<ILogger<QueueService>>();
        var jobLogger = Mock.Of<ILogger<JobService>>();
        
        var printerService = new PrinterService(context, printerLogger);
        var queueService = new QueueService(context, queueLogger);
        var jobService = new JobService(context, jobLogger);
        
        var printer = await printerService.CreateAsync(new CreatePrinterRequest { Name = "Test Printer" });
        var queue = await queueService.CreateAsync(new CreateQueueRequest 
        { 
            Name = "Test Queue", 
            PrinterId = printer.Id 
        });
        
        return (printerService, queueService, jobService, queue.Id);
    }

    [Fact]
    public async Task CreateJob_WithValidQueueId_ShouldCreateJob()
    {
        // Arrange
        var (_, _, jobService, queueId) = await SetupServicesWithQueue();

        var request = new CreateJobRequest
        {
            QueueId = queueId,
            DocumentName = "test.pdf",
            Pages = 10,
            SubmittedBy = "testuser"
        };

        // Act
        var result = await jobService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test.pdf", result.DocumentName);
        Assert.Equal(10, result.Pages);
        Assert.Equal("testuser", result.SubmittedBy);
        Assert.Equal("Queued", result.Status);
    }

    [Fact]
    public async Task CreateJob_WithInvalidQueueId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var logger = Mock.Of<ILogger<JobService>>();
        var service = new JobService(context, logger);

        var request = new CreateJobRequest
        {
            QueueId = Guid.NewGuid(), // Non-existent queue
            DocumentName = "test.pdf",
            Pages = 10
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CreateAsync(request));
    }

    [Fact]
    public async Task CancelJob_WhenJobIsQueued_ShouldCancelJob()
    {
        // Arrange
        var (_, _, jobService, queueId) = await SetupServicesWithQueue();

        var job = await jobService.CreateAsync(new CreateJobRequest
        {
            QueueId = queueId,
            DocumentName = "cancel-me.pdf",
            Pages = 5
        });

        // Act
        var result = await jobService.CancelAsync(job.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Canceled", result.Status);
    }

    [Fact]
    public async Task CancelJob_WhenJobNotExists_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var logger = Mock.Of<ILogger<JobService>>();
        var service = new JobService(context, logger);

        // Act
        var result = await service.CancelAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllJobs_ShouldReturnPaginatedResults()
    {
        // Arrange
        var (_, _, jobService, queueId) = await SetupServicesWithQueue();

        // Create 15 jobs
        for (int i = 0; i < 15; i++)
        {
            await jobService.CreateAsync(new CreateJobRequest
            {
                QueueId = queueId,
                DocumentName = $"doc{i}.pdf",
                Pages = i + 1
            });
        }

        // Act - Get first page with 10 items
        var result = await jobService.GetAllAsync(1, 10);

        // Assert
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(15, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetAllJobs_WithStatusFilter_ShouldFilterByStatus()
    {
        // Arrange
        var (_, _, jobService, queueId) = await SetupServicesWithQueue();

        // Create jobs
        await jobService.CreateAsync(new CreateJobRequest
        {
            QueueId = queueId,
            DocumentName = "queued1.pdf",
            Pages = 5
        });

        // Act - Filter by Queued status
        var result = await jobService.GetAllAsync(1, 10, "Queued");

        // Assert
        Assert.Single(result.Items);
        Assert.All(result.Items, job => Assert.Equal("Queued", job.Status));
    }

    [Fact]
    public async Task GetAllJobs_WithQueueIdFilter_ShouldFilterByQueue()
    {
        // Arrange
        var (_, _, jobService, queueId) = await SetupServicesWithQueue();

        await jobService.CreateAsync(new CreateJobRequest
        {
            QueueId = queueId,
            DocumentName = "filtered.pdf",
            Pages = 5
        });

        // Act
        var result = await jobService.GetAllAsync(1, 10, null, queueId);

        // Assert
        Assert.Single(result.Items);
        Assert.All(result.Items, job => Assert.Equal(queueId, job.QueueId));
    }
}
