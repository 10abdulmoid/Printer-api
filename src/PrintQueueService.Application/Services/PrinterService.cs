using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PrintQueueService.Application.Data;
using PrintQueueService.Application.DTOs.Printers;
using PrintQueueService.Application.Interfaces;
using PrintQueueService.Domain.Entities;
using PrintQueueService.Domain.Enums;

namespace PrintQueueService.Application.Services;

public class PrinterService : IPrinterService
{
    private readonly AppDbContext _context;
    private readonly ILogger<PrinterService> _logger;

    public PrinterService(AppDbContext context, ILogger<PrinterService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PrinterResponse> CreateAsync(CreatePrinterRequest request)
    {
        var status = PrinterStatus.Online;
        if (!string.IsNullOrEmpty(request.Status))
        {
            if (!Enum.TryParse<PrinterStatus>(request.Status, true, out status))
            {
                status = PrinterStatus.Online;
            }
        }

        var printer = new Printer
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Location = request.Location,
            Status = status,
            Capabilities = request.Capabilities
        };

        _context.Printers.Add(printer);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created printer {PrinterId} with name {PrinterName}", printer.Id, printer.Name);

        return MapToResponse(printer);
    }

    public async Task<IEnumerable<PrinterResponse>> GetAllAsync()
    {
        var printers = await _context.Printers
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync();

        return printers.Select(MapToResponse);
    }

    public async Task<PrinterResponse?> GetByIdAsync(Guid id)
    {
        var printer = await _context.Printers
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return printer == null ? null : MapToResponse(printer);
    }

    public async Task<PrinterResponse?> UpdateStatusAsync(Guid id, UpdatePrinterStatusRequest request)
    {
        var printer = await _context.Printers.FindAsync(id);
        if (printer == null)
        {
            return null;
        }

        if (!Enum.TryParse<PrinterStatus>(request.Status, true, out var status))
        {
            throw new ArgumentException($"Invalid status value: {request.Status}");
        }

        printer.Status = status;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated printer {PrinterId} status to {Status}", id, status);

        return MapToResponse(printer);
    }

    private static PrinterResponse MapToResponse(Printer printer) => new()
    {
        Id = printer.Id,
        Name = printer.Name,
        Location = printer.Location,
        Status = printer.Status.ToString(),
        Capabilities = printer.Capabilities,
        CreatedAtUtc = printer.CreatedAtUtc,
        UpdatedAtUtc = printer.UpdatedAtUtc
    };
}
