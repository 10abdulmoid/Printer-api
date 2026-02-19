using Microsoft.Extensions.Logging;
using Moq;
using PrintQueueService.Application.DTOs.Printers;
using PrintQueueService.Application.Services;
using PrintQueueService.Domain.Enums;
using PrintQueueService.Tests.TestHelpers;
using Xunit;

namespace PrintQueueService.Tests.Services;

public class PrinterServiceTests
{
    [Fact]
    public async Task CreatePrinter_WithValidName_ShouldCreatePrinter()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var logger = Mock.Of<ILogger<PrinterService>>();
        var service = new PrinterService(context, logger);
        
        var request = new CreatePrinterRequest
        {
            Name = "Test Printer",
            Location = "Room 101",
            Capabilities = "Color,Duplex"
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Test Printer", result.Name);
        Assert.Equal("Room 101", result.Location);
        Assert.Equal("Online", result.Status);
        Assert.Equal("Color,Duplex", result.Capabilities);
    }

    [Fact]
    public async Task CreatePrinter_WithOfflineStatus_ShouldCreateOfflinePrinter()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var logger = Mock.Of<ILogger<PrinterService>>();
        var service = new PrinterService(context, logger);
        
        var request = new CreatePrinterRequest
        {
            Name = "Offline Printer",
            Status = "Offline"
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.Equal("Offline", result.Status);
    }

    [Fact]
    public async Task GetById_WhenPrinterExists_ShouldReturnPrinter()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var logger = Mock.Of<ILogger<PrinterService>>();
        var service = new PrinterService(context, logger);
        
        var createRequest = new CreatePrinterRequest { Name = "Find Me" };
        var created = await service.CreateAsync(createRequest);

        // Act
        var result = await service.GetByIdAsync(created.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("Find Me", result.Name);
    }

    [Fact]
    public async Task GetById_WhenPrinterNotExists_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var logger = Mock.Of<ILogger<PrinterService>>();
        var service = new PrinterService(context, logger);

        // Act
        var result = await service.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateStatus_WhenPrinterExists_ShouldUpdateStatus()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var logger = Mock.Of<ILogger<PrinterService>>();
        var service = new PrinterService(context, logger);
        
        var createRequest = new CreatePrinterRequest { Name = "Status Printer" };
        var created = await service.CreateAsync(createRequest);

        // Act
        var result = await service.UpdateStatusAsync(created.Id, new UpdatePrinterStatusRequest { Status = "Offline" });

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Offline", result.Status);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllPrinters()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var logger = Mock.Of<ILogger<PrinterService>>();
        var service = new PrinterService(context, logger);
        
        await service.CreateAsync(new CreatePrinterRequest { Name = "Printer 1" });
        await service.CreateAsync(new CreatePrinterRequest { Name = "Printer 2" });

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }
}
