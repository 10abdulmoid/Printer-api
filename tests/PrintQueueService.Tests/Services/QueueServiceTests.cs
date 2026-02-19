using Microsoft.Extensions.Logging;
using Moq;
using PrintQueueService.Application.DTOs.Printers;
using PrintQueueService.Application.DTOs.Queues;
using PrintQueueService.Application.Services;
using PrintQueueService.Tests.TestHelpers;
using Xunit;

namespace PrintQueueService.Tests.Services;

public class QueueServiceTests
{
    [Fact]
    public async Task CreateQueue_WithValidPrinterId_ShouldCreateQueue()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var printerLogger = Mock.Of<ILogger<PrinterService>>();
        var queueLogger = Mock.Of<ILogger<QueueService>>();
        var printerService = new PrinterService(context, printerLogger);
        var queueService = new QueueService(context, queueLogger);

        var printer = await printerService.CreateAsync(new CreatePrinterRequest { Name = "Test Printer" });

        var request = new CreateQueueRequest
        {
            Name = "Test Queue",
            PrinterId = printer.Id
        };

        // Act
        var result = await queueService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Queue", result.Name);
        Assert.Equal(printer.Id, result.PrinterId);
        Assert.False(result.IsPaused);
    }

    [Fact]
    public async Task CreateQueue_WithInvalidPrinterId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var logger = Mock.Of<ILogger<QueueService>>();
        var service = new QueueService(context, logger);

        var request = new CreateQueueRequest
        {
            Name = "Test Queue",
            PrinterId = Guid.NewGuid() // Non-existent printer
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CreateAsync(request));
    }

    [Fact]
    public async Task UpdatePauseStatus_ShouldTogglePauseState()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var printerLogger = Mock.Of<ILogger<PrinterService>>();
        var queueLogger = Mock.Of<ILogger<QueueService>>();
        var printerService = new PrinterService(context, printerLogger);
        var queueService = new QueueService(context, queueLogger);

        var printer = await printerService.CreateAsync(new CreatePrinterRequest { Name = "Test Printer" });
        var queue = await queueService.CreateAsync(new CreateQueueRequest 
        { 
            Name = "Test Queue", 
            PrinterId = printer.Id 
        });

        // Act
        var result = await queueService.UpdatePauseStatusAsync(queue.Id, new UpdateQueuePauseRequest { IsPaused = true });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsPaused);
    }
}
