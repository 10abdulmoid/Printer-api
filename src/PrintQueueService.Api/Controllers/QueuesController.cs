using Microsoft.AspNetCore.Mvc;
using PrintQueueService.Application.DTOs.Queues;
using PrintQueueService.Application.Interfaces;

namespace PrintQueueService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class QueuesController : ControllerBase
{
    private readonly IQueueService _queueService;
    private readonly ILogger<QueuesController> _logger;

    public QueuesController(IQueueService queueService, ILogger<QueuesController> logger)
    {
        _queueService = queueService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new queue
    /// </summary>
    /// <param name="request">Queue creation request</param>
    /// <returns>The created queue</returns>
    /// <response code="201">Returns the newly created queue</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the printer is not found</response>
    [HttpPost]
    [ProducesResponseType(typeof(QueueResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QueueResponse>> Create([FromBody] CreateQueueRequest request)
    {
        var queue = await _queueService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = queue.Id }, queue);
    }

    /// <summary>
    /// Gets all queues
    /// </summary>
    /// <returns>List of all queues</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<QueueResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<QueueResponse>>> GetAll()
    {
        var queues = await _queueService.GetAllAsync();
        return Ok(queues);
    }

    /// <summary>
    /// Gets a queue by ID
    /// </summary>
    /// <param name="id">Queue ID</param>
    /// <returns>The queue</returns>
    /// <response code="200">Returns the queue</response>
    /// <response code="404">If the queue is not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(QueueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QueueResponse>> GetById(Guid id)
    {
        var queue = await _queueService.GetByIdAsync(id);
        if (queue == null)
        {
            return NotFound(new { message = $"Queue with ID {id} not found" });
        }
        return Ok(queue);
    }

    /// <summary>
    /// Updates a queue's pause status
    /// </summary>
    /// <param name="id">Queue ID</param>
    /// <param name="request">Pause status update request</param>
    /// <returns>The updated queue</returns>
    /// <response code="200">Returns the updated queue</response>
    /// <response code="404">If the queue is not found</response>
    [HttpPatch("{id:guid}/pause")]
    [ProducesResponseType(typeof(QueueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QueueResponse>> UpdatePauseStatus(Guid id, [FromBody] UpdateQueuePauseRequest request)
    {
        var queue = await _queueService.UpdatePauseStatusAsync(id, request);
        if (queue == null)
        {
            return NotFound(new { message = $"Queue with ID {id} not found" });
        }
        return Ok(queue);
    }
}
