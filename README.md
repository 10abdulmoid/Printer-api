# Print Queue Service API

A production-quality .NET 8 ASP.NET Core REST API for managing printers, print queues, and print jobs. This service mimics the core workflow of an enterprise print service backend by handling printer registration, queue management, job submission, and background job processing.

## Features

- **Printer Management**: Register printers with capabilities (Color, Duplex, etc.) and track their status (Online/Offline)
- **Queue Management**: Create print queues mapped to printers with pause/resume functionality
- **Job Processing**: Submit print jobs, track status, and cancel pending jobs
- **Background Processing**: Automated job processor that simulates printing (Queued → Processing → Completed/Failed)
- **RESTful API**: Clean REST endpoints with proper HTTP status codes
- **Swagger/OpenAPI**: Interactive API documentation
- **Input Validation**: DataAnnotations-based request validation
- **Error Handling**: Global exception handling with trace IDs
- **Pagination**: Support for paginated job listings with filtering
- **Clean Architecture**: Layered architecture (API, Application, Domain, Infrastructure)
- **Unit Tests**: Comprehensive xUnit tests with in-memory database

## Tech Stack

- **Framework**: .NET 8, ASP.NET Core Web API
- **Database**: SQLite with Entity Framework Core
- **API Documentation**: Swagger/OpenAPI (Swashbuckle)
- **Validation**: DataAnnotations
- **Testing**: xUnit + Moq + EF Core InMemory
- **Logging**: Built-in ILogger with structured logging

## Project Structure

```
PrintQueueService.sln
├── src/
│   ├── PrintQueueService.Api/          # REST API controllers, middleware
│   ├── PrintQueueService.Application/  # Services, DTOs, interfaces, DbContext
│   ├── PrintQueueService.Domain/       # Entities, enums, base classes
│   └── PrintQueueService.Infrastructure/ # SQLite configuration
└── tests/
    └── PrintQueueService.Tests/        # Unit tests
```

## Prerequisites

- .NET 8 SDK
- macOS, Linux, or Windows

## Getting Started

### 1. Clone and Navigate

```bash
cd /path/to/printer-api
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build

```bash
dotnet build
```

### 4. Run the API

```bash
dotnet run --project src/PrintQueueService.Api
```

The API will start at `http://localhost:5000`

### 5. Access Swagger UI

Open your browser and navigate to: `http://localhost:5000`

## API Endpoints

### Printers

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/printers` | Create a new printer |
| GET | `/api/printers` | Get all printers |
| GET | `/api/printers/{id}` | Get printer by ID |
| PATCH | `/api/printers/{id}/status` | Update printer status |

### Queues

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/queues` | Create a new queue |
| GET | `/api/queues` | Get all queues |
| GET | `/api/queues/{id}` | Get queue by ID |
| PATCH | `/api/queues/{id}/pause` | Pause/resume queue |

### Jobs

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/jobs` | Submit a print job |
| GET | `/api/jobs` | Get jobs (paginated, filterable) |
| GET | `/api/jobs/{id}` | Get job by ID |
| POST | `/api/jobs/{id}/cancel` | Cancel a job |

## Sample cURL Commands

### Create a Printer

```bash
curl -X POST http://localhost:5000/api/printers \
  -H "Content-Type: application/json" \
  -d '{"name": "Office Printer", "location": "Room 101", "capabilities": "Color,Duplex"}'
```

### Create a Queue

```bash
curl -X POST http://localhost:5000/api/queues \
  -H "Content-Type: application/json" \
  -d '{"name": "Main Queue", "printerId": "<printer-id-from-above>"}'
```

### Submit a Print Job

```bash
curl -X POST http://localhost:5000/api/jobs \
  -H "Content-Type: application/json" \
  -d '{"queueId": "<queue-id-from-above>", "documentName": "report.pdf", "pages": 10, "submittedBy": "john.doe"}'
```

### Get Jobs with Pagination

```bash
curl "http://localhost:5000/api/jobs?page=1&pageSize=10&status=Queued"
```

### Cancel a Job

```bash
curl -X POST http://localhost:5000/api/jobs/<job-id>/cancel
```

## Background Job Processing

The service includes a background processor that:

1. Runs every 3 seconds
2. Picks up to 5 queued jobs from active (non-paused) queues with online printers
3. Marks jobs as Processing, simulates work, then marks as Completed
4. Jobs with > 300 pages are marked as Failed (demo simulation)

## Running Tests

```bash
dotnet test
```

Tests use EF Core InMemory provider for fast, isolated execution.

## Database

The application uses SQLite. The database file (`printqueue.db`) is created automatically in the API project directory on first run. Migrations are applied automatically at startup.

### Manual Migration Commands (if needed)

```bash
# Add a new migration
dotnet ef migrations add MigrationName --project src/PrintQueueService.Application --startup-project src/PrintQueueService.Api --output-dir Data/Migrations

# Update database
dotnet ef database update --project src/PrintQueueService.Application --startup-project src/PrintQueueService.Api
```

## Configuration

Edit `src/PrintQueueService.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=printqueue.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## Error Responses

All errors return a consistent JSON format:

```json
{
  "traceId": "0HN6VKJL1ABCD:00000001",
  "message": "Error description"
}
```

## Status Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request (validation error) |
| 404 | Not Found |
| 409 | Conflict (e.g., canceling completed job) |
| 500 | Internal Server Error |

## License

MIT
