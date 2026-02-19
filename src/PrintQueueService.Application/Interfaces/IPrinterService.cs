using PrintQueueService.Application.DTOs.Printers;

namespace PrintQueueService.Application.Interfaces;

public interface IPrinterService
{
    Task<PrinterResponse> CreateAsync(CreatePrinterRequest request);
    Task<IEnumerable<PrinterResponse>> GetAllAsync();
    Task<PrinterResponse?> GetByIdAsync(Guid id);
    Task<PrinterResponse?> UpdateStatusAsync(Guid id, UpdatePrinterStatusRequest request);
}
