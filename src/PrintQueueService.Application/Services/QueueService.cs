using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PrintQueueService.Application.Data;
using PrintQueueService.Application.DTOs.Queues;
using PrintQueueService.Application.Interfaces;
using PrintQueueService.Domain.Entities;

namespace PrintQueueService.Application.Services;

public class QueueService : IQueueService
{
    private readonly AppDbContext _context;
    private readonly ILogger<QueueService> _logger;

    public QueueService(AppDbContext context, ILogger<QueueService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<QueueResponse> CreateAsync(CreateQueueRequest request)
    {
        // Verify printer exists
        var printerExists = await _context.Printers.AnyAsync(p => p.Id == request.PrinterId);
        if (!printerExists)
        {
            throw new KeyNotFoundException($"Printer with ID {request.PrinterId} not found");
        }

        var queue = new Queue
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            PrinterId = request.PrinterId,
            IsPaused = false
        };

        _context.Queues.Add(queue);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created queue {QueueId} with name {QueueName} for printer {PrinterId}", 
            queue.Id, queue.Name, queue.PrinterId);

        return MapToResponse(queue);
    }

    public async Task<IEnumerable<QueueResponse>> GetAllAsync()
    {
        var queues = await _context.Queues
            .AsNoTracking()
            .OrderBy(q => q.Name)
            .ToListAsync();

        return queues.Select(MapToResponse);
    }

    public async Task<QueueResponse?> GetByIdAsync(Guid id)
    {
        var queue = await _context.Queues
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id);

        return queue == null ? null : MapToResponse(queue);
    }

    public async Task<QueueResponse?> UpdatePauseStatusAsync(Guid id, UpdateQueuePauseRequest request)
    {
        var queue = await _context.Queues.FindAsync(id);
        if (queue == null)
        {
            return null;
        }

        queue.IsPaused = request.IsPaused;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated queue {QueueId} pause status to {IsPaused}", id, request.IsPaused);

        return MapToResponse(queue);
    }

    private static QueueResponse MapToResponse(Queue queue) => new()
    {
        Id = queue.Id,
        Name = queue.Name,
        PrinterId = queue.PrinterId,
        IsPaused = queue.IsPaused,
        CreatedAtUtc = queue.CreatedAtUtc,
        UpdatedAtUtc = queue.UpdatedAtUtc
    };
}
