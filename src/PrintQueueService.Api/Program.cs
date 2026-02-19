using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PrintQueueService.Api.Middleware;
using PrintQueueService.Application;
using PrintQueueService.Application.Data;
using PrintQueueService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Print Queue Service API",
        Version = "v1",
        Description = "A .NET 8 ASP.NET Core REST API for managing printers, queues, and print jobs"
    });
});

// Add custom services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

// Apply migrations and create database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline
app.UseExceptionHandling();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Print Queue Service API v1");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();

app.Run();
