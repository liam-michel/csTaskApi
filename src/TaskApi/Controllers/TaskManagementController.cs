using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApi.Models;
using TaskApi.Models.Dtos;
using TaskApi.Services.Orchestrators;

namespace TaskApi.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TaskManagementController : ControllerBase, IDisposable
{
  private readonly ITaskManagementOrchestrator _orchestrator;
  private readonly ILogger<TaskManagementController> _logger;
  private readonly IDisposable? _logScope;

  public TaskManagementController(
    ITaskManagementOrchestrator orchestrator,
    ILogger<TaskManagementController> logger)
  {
    _orchestrator = orchestrator;
    _logger = logger;
    _logScope = logger.BeginScope("{Layer}", "TaskController");
  }

  public void Dispose()
  {
    _logScope?.Dispose();
    GC.SuppressFinalize(this);
  }

  private int GetCurrentUserId() =>
    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

  [HttpGet]
  public async Task<ActionResult<List<TaskItem>>> GetAll()
  {
    var tasks = await _orchestrator.GetAllAsync(GetCurrentUserId());
    return Ok(tasks);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<TaskItem>> GetById(int id)
  {
    var task = await _orchestrator.GetByIdAsync(id, GetCurrentUserId());
    if (task == null) return NotFound();
    return Ok(task);
  }

  [HttpPost]
  public async Task<ActionResult<TaskItem>> Create([FromBody] CreateTaskRequest request)
  {
    // FluentValidation auto-validates, returns BadRequest if invalid
    // This only runs if validation passed

    request.UserId = GetCurrentUserId();
    _logger.LogInformation("Creating task: {Title}", request.Title);
    var task = await _orchestrator.CreateAsync(request);
    return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
  }

  [HttpPut]
  public async Task<ActionResult<TaskItem>> Update([FromBody] UpdateTaskRequest request)
  {
    // FluentValidation auto-validates, returns BadRequest if invalid
    // This only runs if validation passed

    _logger.LogInformation("Updating task {TaskId}", request.Id);
    var updated = await _orchestrator.UpdateAsync(request, GetCurrentUserId());
    if (updated == null) return NotFound();
    return Ok(updated);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id)
  {
    _logger.LogInformation("Deleting task {TaskId}", id);
    var deleted = await _orchestrator.DeleteAsync(id, GetCurrentUserId());
    if (!deleted) return NotFound();
    return NoContent();
  }
}
