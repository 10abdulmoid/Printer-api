using Microsoft.Extensions.DependencyInjection;
using PrintQueueService.Application.Interfaces;
using PrintQueueService.Application.Services;

namespace PrintQueueService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPrinterService, PrinterService>();
        services.AddScoped<IQueueService, QueueService>();
        services.AddScoped<IJobService, JobService>();
        services.AddHostedService<PrintJobProcessorService>();

        return services;
    }
}
