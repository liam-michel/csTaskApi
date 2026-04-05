using FluentValidation;
using TaskApi.Models.Dtos;

namespace TaskApi.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
  public CreateUserRequestValidator()
  {
    RuleFor(x => x.Username)
      .NotEmpty().WithMessage("Username is required")
      .Length(1, 50).WithMessage("Username must be 1-50 characters");

    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email is required")
      .EmailAddress().WithMessage("Invalid email format")
      .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

    RuleFor(x => x.PasswordHash)
      .NotEmpty().WithMessage("Password is required")
      .MinimumLength(8).WithMessage("Password must be at least 8 characters")
      .MaximumLength(255).WithMessage("Password must not exceed 255 characters");
  }
}

public class UpdateUserDetailsRequestValidator : AbstractValidator<UpdateUserDetailsRequest>
{
  public UpdateUserDetailsRequestValidator()
  {
    RuleFor(x => x.Id)
      .GreaterThan(0).WithMessage("Id must be greater than 0");

    RuleFor(x => x.Username)
      .NotEmpty().WithMessage("Username is required")
      .Length(1, 50).WithMessage("Username must be 1-50 characters");

    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email is required")
      .EmailAddress().WithMessage("Invalid email format")
      .MaximumLength(255).WithMessage("Email must not exceed 255 characters");
  }
}

public class UpdateUserPasswordRequestValidator : AbstractValidator<UpdateUserPasswordRequest>
{
  public UpdateUserPasswordRequestValidator()
  {
    RuleFor(x => x.Id)
      .GreaterThan(0).WithMessage("Id must be greater than 0");

    RuleFor(x => x.PasswordHash)
      .NotEmpty().WithMessage("Password is required")
      .MinimumLength(8).WithMessage("Password must be at least 8 characters")
      .MaximumLength(255).WithMessage("Password must not exceed 255 characters");
  }
}
