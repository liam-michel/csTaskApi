using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApi.Models;
using TaskApi.Models.Dtos;
using TaskApi.Services.Orchestrators;

namespace TaskApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserManagementController : ControllerBase, IDisposable
{
  private readonly IUserManagementOrchestrator _orchestrator;
  private readonly ILogger<UserManagementController> _logger;
  private readonly IDisposable? _logScope;

  public UserManagementController(
    IUserManagementOrchestrator orchestrator,
    ILogger<UserManagementController> logger)
  {
    _orchestrator = orchestrator;
    _logger = logger;
    _logScope = logger.BeginScope("{Layer}", "UserController");
  }

  public void Dispose()
  {
    _logScope?.Dispose();
    GC.SuppressFinalize(this);
  }

  private int GetCurrentUserId() =>
    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

  [HttpGet]
  public async Task<ActionResult<List<User>>> GetAll()
  {
    var users = await _orchestrator.GetAllUsersAsync();
    return Ok(users);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<User>> GetById(int id)
  {
    var user = await _orchestrator.GetUserByIdAsync(id);
    if (user == null) return NotFound();
    return Ok(user);
  }

  [HttpPost]
  public async Task<ActionResult<User>> Create([FromBody] CreateUserRequest request)
  {
    // FluentValidation auto-validates, returns BadRequest if invalid
    // This only runs if validation passed

    _logger.LogInformation("Creating user: {Username}", request.Username);
    var user = await _orchestrator.AddUserAsync(request);
    return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
  }

  [HttpPut]
  public async Task<ActionResult<User>> UpdateDetails([FromBody] UpdateUserDetailsRequest request)
  {
    // FluentValidation auto-validates, returns BadRequest if invalid
    // This only runs if validation passed

    try
    {
      _logger.LogInformation("Updating details for user {UserId}", request.Id);
      var updated = await _orchestrator.UpdateUserDetailsAsync(request, GetCurrentUserId());
      return Ok(updated);
    }
    catch (UnauthorizedAccessException)
    {
      return Forbid();
    }
  }

  [HttpPut("password")]
  public async Task<ActionResult<User>> UpdatePassword([FromBody] UpdateUserPasswordRequest request)
  {
    // FluentValidation auto-validates, returns BadRequest if invalid
    // This only runs if validation passed

    try
    {
      _logger.LogInformation("Updating password for user {UserId}", request.Id);
      var updated = await _orchestrator.UpdateUserPasswordAsync(request, GetCurrentUserId());
      return Ok(updated);
    }
    catch (UnauthorizedAccessException)
    {
      return Forbid();
    }
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id)
  {
    try
    {
      _logger.LogInformation("Deleting user {UserId}", id);
      var deleted = await _orchestrator.DeleteUserAsync(id, GetCurrentUserId());
      if (!deleted) return NotFound();
      return NoContent();
    }
    catch (UnauthorizedAccessException)
    {
      return Forbid();
    }
  }
}
