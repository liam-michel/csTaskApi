using Microsoft.AspNetCore.Mvc;
using TaskApi.Models.Dtos;
using TaskApi.Services;

namespace TaskApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase, IDisposable
{
  private readonly IAuthService _authService;
  private readonly ILogger<AuthController> _logger;
  private readonly IDisposable? _logScope;

  public AuthController(IAuthService authService, ILogger<AuthController> logger)
  {
    _authService = authService;
    _logger = logger;
    _logScope = logger.BeginScope("{Layer}", "AuthController");
  }

  public void Dispose()
  {
    _logScope?.Dispose();
    GC.SuppressFinalize(this);
  }

  [HttpPost("register")]
  public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
  {
    // FluentValidation auto-validates, returns BadRequest if invalid
    // This only runs if validation passed

    _logger.LogInformation("Registering user: {Email}", request.Email);
    var response = await _authService.RegisterAsync(request);
    return Ok(response);
  }

  [HttpPost("login")]
  public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
  {
    // FluentValidation auto-validates, returns BadRequest if invalid
    // This only runs if validation passed

    _logger.LogInformation("Login attempt: {Email}", request.Email);
    var response = await _authService.LoginAsync(request);
    return Ok(response);
  }
}
