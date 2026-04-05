using FluentValidation;
using TaskApi.Models.Dtos;

namespace TaskApi.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
  public LoginRequestValidator()
  {
    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email is required")
      .EmailAddress().WithMessage("Invalid email format");

    RuleFor(x => x.Password)
      .NotEmpty().WithMessage("Password is required");
  }
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
  public RegisterRequestValidator()
  {
    RuleFor(x => x.Username)
      .NotEmpty().WithMessage("Username is required")
      .Length(1, 50).WithMessage("Username must be 1-50 characters");

    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email is required")
      .EmailAddress().WithMessage("Invalid email format")
      .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

    RuleFor(x => x.Password)
      .NotEmpty().WithMessage("Password is required")
      .MinimumLength(8).WithMessage("Password must be at least 8 characters")
      .MaximumLength(255).WithMessage("Password must not exceed 255 characters");
  }
}
