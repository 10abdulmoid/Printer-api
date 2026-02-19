using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PrintQueueService.Application.DTOs.Jobs;
using PrintQueueService.Application.Interfaces;

namespace PrintQueueService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IJobService jobService, ILogger<JobsController> logger)
    {
        _jobService = jobService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new print job
    /// </summary>
    /// <param name="request">Job creation request</param>
    /// <returns>The created job</returns>
    /// <response code="201">Returns the newly created job</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the queue is not found</response>
    [HttpPost]
    [ProducesResponseType(typeof(JobResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobResponse>> Create([FromBody] CreateJobRequest request)
    {
        var job = await _jobService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
    }

    /// <summary>
    /// Gets a job by ID
    /// </summary>
    /// <param name="id">Job ID</param>
    /// <returns>The job</returns>
    /// <response code="200">Returns the job</response>
    /// <response code="404">If the job is not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(JobResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobResponse>> GetById(Guid id)
    {
        var job = await _jobService.GetByIdAsync(id);
        if (job == null)
        {
            return NotFound(new { message = $"Job with ID {id} not found" });
        }
        return Ok(job);
    }

    /// <summary>
    /// Gets all jobs with pagination and optional filtering
    /// </summary>
    /// <param name="page">Page number (default: 1, min: 1)</param>
    /// <param name="pageSize">Page size (default: 10, range: 1-100)</param>
    /// <param name="status">Optional status filter (Queued, Processing, Completed, Failed, Canceled)</param>
    /// <param name="queueId">Optional queue ID filter</param>
    /// <returns>Paginated list of jobs</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedJobsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedJobsResponse>> GetAll(
        [FromQuery][Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")] int page = 1,
        [FromQuery][Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] Guid? queueId = null)
    {
        var result = await _jobService.GetAllAsync(page, pageSize, status, queueId);
        return Ok(result);
    }

    /// <summary>
    /// Cancels a print job
    /// </summary>
    /// <param name="id">Job ID</param>
    /// <returns>The canceled job</returns>
    /// <response code="200">Returns the canceled job</response>
    /// <response code="404">If the job is not found</response>
    /// <response code="409">If the job cannot be canceled (already completed/failed/canceled)</response>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(JobResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<JobResponse>> Cancel(Guid id)
    {
        var job = await _jobService.CancelAsync(id);
        if (job == null)
        {
            return NotFound(new { message = $"Job with ID {id} not found" });
        }
        return Ok(job);
    }
}
