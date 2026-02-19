# Print Queue Service API
![Printer API Architecture](https://github.com/10abdulmoid/Printer-api/blob/main/Print%20Queue%20Service%20API.png)

A production-quality .NET 8 ASP.NET Core REST API for managing printers, print queues, and print jobs. This service mimics the core workflow of an enterprise print service backend by handling printer registration, queue management, job submission, and background job processing.

## What This Project Does (Simple Explanation)

This project simulates how a **print management system** works (like what companies use to manage office printers). Think of it as the "backend brain" for a printing service.

### 🖨️ What It Manages

**1. Printers**
- Register printers with names like "Office Printer Floor 2"
- Set their location (e.g., "Room 101")
- Track if they're **Online** (working) or **Offline** (not available)
- Store capabilities like "Color, Duplex" (double-sided printing)

**2. Queues**
- Each printer has a queue (like a waiting line)
- Jobs wait in the queue until the printer is ready
- You can **pause** a queue (stops processing) or **resume** it

**3. Print Jobs**
- Users submit documents to print (e.g., "report.pdf", 10 pages)
- Each job tracks: document name, pages, who submitted it
- Status flow: `Queued → Processing → Completed/Failed`

### ⚙️ How It Works

1. **Submit a job** → Status = "Queued"
2. **Background processor** (runs every 3 seconds):
   - Picks up queued jobs from active queues with online printers
   - Changes status to "Processing"
   - Simulates printing (small delay)
   - Marks as "Completed" ✅
3. **Failure simulation**: Jobs with >300 pages → "Failed" ❌

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
- **Unit Tests**: 16 comprehensive xUnit tests with in-memory database

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
    └── PrintQueueService.Tests/        # Unit tests (16 tests)
```

## Prerequisites

- .NET 8 SDK
- macOS, Linux, or Windows

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/10abdulmoid/Printer-api.git
cd Printer-api
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
dotnet run --project src/PrintQueueService.Api --urls "http://localhost:5050"
```

> ⚠️ **Note for macOS users**: Use port 5050 instead of 5000. Port 5000 is blocked by AirPlay Receiver on macOS and Chrome will show "Access denied" error.

### 5. Access Swagger UI

Open your browser and navigate to: **http://localhost:5050**

You'll see an interactive API documentation where you can test all endpoints.

## 🧪 Testing the API (Step-by-Step Guide)

### Using Swagger UI (Recommended)

#### Step 1: Create a Printer
1. In Swagger UI, find **POST /api/Printers** and click on it
2. Click **"Try it out"**
3. Enter this JSON:
```json
{
  "name": "Office Printer",
  "location": "Room 101",
  "capabilities": "Color,Duplex"
}
```
4. Click **"Execute"**
5. **Copy the `id`** from the response - you'll need it!

#### Step 2: Create a Queue
1. Click on **POST /api/Queues**
2. Click **"Try it out"**
3. Enter (replace with your printer ID):
```json
{
  "name": "Main Queue",
  "printerId": "YOUR-PRINTER-ID-HERE"
}
```
4. Click **"Execute"**
5. **Copy the queue `id`**

#### Step 3: Submit a Print Job
1. Click on **POST /api/Jobs**
2. Click **"Try it out"**
3. Enter (replace with your queue ID):
```json
{
  "queueId": "YOUR-QUEUE-ID-HERE",
  "documentName": "report.pdf",
  "pages": 50,
  "submittedBy": "john.doe"
}
```
4. Click **"Execute"**
5. Note the job `id` and status is `"Queued"`

#### Step 4: Check Job Status (Wait 3-5 seconds)
1. Click on **GET /api/Jobs/{id}**
2. Click **"Try it out"**
3. Paste your job ID
4. Click **"Execute"**
5. Status should now be `"Completed"` ✅

#### Step 5: Test Failure Simulation
Submit a job with more than 300 pages:
```json
{
  "queueId": "YOUR-QUEUE-ID-HERE",
  "documentName": "large-document.pdf",
  "pages": 400,
  "submittedBy": "john.doe"
}
```
After a few seconds, check the job - it will be `"Failed"` with error message: `"Job too large for demo processor (max 300 pages)"`

### Other Things to Try
- **GET /api/Printers** - List all printers
- **PATCH /api/Printers/{id}/status** - Set printer to `{"status": "Offline"}`
- **PATCH /api/Queues/{id}/pause** - Pause queue with `{"isPaused": true}`
- **POST /api/Jobs/{id}/cancel** - Cancel a queued job
- **GET /api/Jobs?status=Completed&page=1&pageSize=10** - Filter jobs

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
curl -X POST http://localhost:5050/api/printers \
  -H "Content-Type: application/json" \
  -d '{"name": "Office Printer", "location": "Room 101", "capabilities": "Color,Duplex"}'
```

### Create a Queue

```bash
curl -X POST http://localhost:5050/api/queues \
  -H "Content-Type: application/json" \
  -d '{"name": "Main Queue", "printerId": "<printer-id-from-above>"}'
```

### Submit a Print Job

```bash
curl -X POST http://localhost:5050/api/jobs \
  -H "Content-Type: application/json" \
  -d '{"queueId": "<queue-id-from-above>", "documentName": "report.pdf", "pages": 10, "submittedBy": "john.doe"}'
```

### Get Jobs with Pagination and Filtering

```bash
curl "http://localhost:5050/api/jobs?page=1&pageSize=10&status=Queued"
```

### Cancel a Job

```bash
curl -X POST http://localhost:5050/api/jobs/<job-id>/cancel
```

## Background Job Processing

The service includes a background processor that:

1. Runs every **3 seconds**
2. Picks up to **5 queued jobs** from active (non-paused) queues with online printers
3. Marks jobs as `Processing`, simulates work, then marks as `Completed`
4. **Jobs with > 300 pages are automatically marked as `Failed`** (demo simulation)

### Processing Rules
- Jobs are only processed if the **printer is Online**
- Jobs are only processed if the **queue is not paused**
- Jobs are processed in **FIFO order** (first in, first out)

## Running Tests

```bash
dotnet test
```

**16 tests** covering:
- Printer creation and validation
- Queue creation (validates printer exists)
- Job submission (validates queue exists)
- Job cancellation (returns 409 if already completed)
- Pagination and filtering

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

## Stopping the Server

```bash
pkill -f dotnet
```

## License

MIT
