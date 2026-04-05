using FluentValidation;
using TaskApi.Models.Dtos;

namespace TaskApi.Validators;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
  public CreateTaskRequestValidator()
  {
    RuleFor(x => x.Title)
      .NotEmpty().WithMessage("Title is required")
      .Length(1, 200).WithMessage("Title must be 1-200 characters");

    RuleFor(x => x.Description)
      .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
  }
}

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
  public UpdateTaskRequestValidator()
  {
    RuleFor(x => x.Id)
      .GreaterThan(0).WithMessage("Id must be greater than 0");

    RuleFor(x => x.Title)
      .NotEmpty().WithMessage("Title is required")
      .Length(1, 200).WithMessage("Title must be 1-200 characters");

    RuleFor(x => x.Description)
      .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
  }
}
