using Microsoft.AspNetCore.Mvc;
using PrintQueueService.Application.DTOs.Printers;
using PrintQueueService.Application.Interfaces;

namespace PrintQueueService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PrintersController : ControllerBase
{
    private readonly IPrinterService _printerService;
    private readonly ILogger<PrintersController> _logger;

    public PrintersController(IPrinterService printerService, ILogger<PrintersController> logger)
    {
        _printerService = printerService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new printer
    /// </summary>
    /// <param name="request">Printer creation request</param>
    /// <returns>The created printer</returns>
    /// <response code="201">Returns the newly created printer</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(PrinterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PrinterResponse>> Create([FromBody] CreatePrinterRequest request)
    {
        var printer = await _printerService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = printer.Id }, printer);
    }

    /// <summary>
    /// Gets all printers
    /// </summary>
    /// <returns>List of all printers</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PrinterResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PrinterResponse>>> GetAll()
    {
        var printers = await _printerService.GetAllAsync();
        return Ok(printers);
    }

    /// <summary>
    /// Gets a printer by ID
    /// </summary>
    /// <param name="id">Printer ID</param>
    /// <returns>The printer</returns>
    /// <response code="200">Returns the printer</response>
    /// <response code="404">If the printer is not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PrinterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PrinterResponse>> GetById(Guid id)
    {
        var printer = await _printerService.GetByIdAsync(id);
        if (printer == null)
        {
            return NotFound(new { message = $"Printer with ID {id} not found" });
        }
        return Ok(printer);
    }

    /// <summary>
    /// Updates a printer's status
    /// </summary>
    /// <param name="id">Printer ID</param>
    /// <param name="request">Status update request</param>
    /// <returns>The updated printer</returns>
    /// <response code="200">Returns the updated printer</response>
    /// <response code="404">If the printer is not found</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(PrinterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PrinterResponse>> UpdateStatus(Guid id, [FromBody] UpdatePrinterStatusRequest request)
    {
        var printer = await _printerService.UpdateStatusAsync(id, request);
        if (printer == null)
        {
            return NotFound(new { message = $"Printer with ID {id} not found" });
        }
        return Ok(printer);
    }
}
