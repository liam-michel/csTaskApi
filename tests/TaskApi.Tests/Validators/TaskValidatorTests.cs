using FluentAssertions;
using TaskApi.Models.Dtos;
using TaskApi.Validators;

namespace TaskApi.Tests.Validators;

public class TaskValidatorTests
{
    private readonly CreateTaskRequestValidator _createValidator = new();
    private readonly UpdateTaskRequestValidator _updateValidator = new();

    // --- CreateTaskRequest ---

    [Fact]
    public void Create_ValidRequest_PassesValidation()
    {
        var request = new CreateTaskRequest { Title = "Fix bug", Description = "Details here", UserId = 1 };
        var result = _createValidator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Create_EmptyTitle_FailsValidation()
    {
        var request = new CreateTaskRequest { Title = "", Description = "Details", UserId = 1 };
        var result = _createValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Title is required");
    }

    [Fact]
    public void Create_TitleTooLong_FailsValidation()
    {
        var request = new CreateTaskRequest { Title = new string('a', 201), UserId = 1 };
        var result = _createValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Title must be 1-200 characters");
    }

    [Fact]
    public void Create_DescriptionTooLong_FailsValidation()
    {
        var request = new CreateTaskRequest { Title = "Fix bug", Description = new string('a', 1001), UserId = 1 };
        var result = _createValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Description must not exceed 1000 characters");
    }

    [Fact]
    public void Create_NoUserId_PassesValidation()
    {
        // UserId is set server-side from JWT, not validated from the request body
        var request = new CreateTaskRequest { Title = "Fix bug" };
        var result = _createValidator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    // --- UpdateTaskRequest ---

    [Fact]
    public void Update_ValidRequest_PassesValidation()
    {
        var request = new UpdateTaskRequest { Id = 1, Title = "Updated title", Description = "Updated desc", IsCompleted = false };
        var result = _updateValidator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Update_InvalidId_FailsValidation()
    {
        var request = new UpdateTaskRequest { Id = 0, Title = "Updated title" };
        var result = _updateValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Id must be greater than 0");
    }

    [Fact]
    public void Update_EmptyTitle_FailsValidation()
    {
        var request = new UpdateTaskRequest { Id = 1, Title = "" };
        var result = _updateValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Title is required");
    }
}
