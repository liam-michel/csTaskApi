using FluentAssertions;
using TaskApi.Models.Dtos;
using TaskApi.Validators;

namespace TaskApi.Tests.Validators;

public class UserValidatorTests
{
    private readonly CreateUserRequestValidator _createValidator = new();
    private readonly UpdateUserDetailsRequestValidator _updateDetailsValidator = new();
    private readonly UpdateUserPasswordRequestValidator _updatePasswordValidator = new();

    // --- CreateUserRequest ---

    [Fact]
    public void Create_ValidRequest_PassesValidation()
    {
        var request = new CreateUserRequest { Username = "liam", Email = "liam@example.com", PasswordHash = "hashedpassword" };
        var result = _createValidator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Create_EmptyUsername_FailsValidation()
    {
        var request = new CreateUserRequest { Username = "", Email = "liam@example.com", PasswordHash = "hashedpassword" };
        var result = _createValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Username is required");
    }

    [Fact]
    public void Create_InvalidEmail_FailsValidation()
    {
        var request = new CreateUserRequest { Username = "liam", Email = "not-an-email", PasswordHash = "hashedpassword" };
        var result = _createValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Invalid email format");
    }

    [Fact]
    public void Create_PasswordTooShort_FailsValidation()
    {
        var request = new CreateUserRequest { Username = "liam", Email = "liam@example.com", PasswordHash = "short" };
        var result = _createValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Password must be at least 8 characters");
    }

    // --- UpdateUserDetailsRequest ---

    [Fact]
    public void UpdateDetails_ValidRequest_PassesValidation()
    {
        var request = new UpdateUserDetailsRequest { Id = 1, Username = "liam", Email = "liam@example.com" };
        var result = _updateDetailsValidator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void UpdateDetails_InvalidId_FailsValidation()
    {
        var request = new UpdateUserDetailsRequest { Id = 0, Username = "liam", Email = "liam@example.com" };
        var result = _updateDetailsValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Id must be greater than 0");
    }

    [Fact]
    public void UpdateDetails_InvalidEmail_FailsValidation()
    {
        var request = new UpdateUserDetailsRequest { Id = 1, Username = "liam", Email = "bad-email" };
        var result = _updateDetailsValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Invalid email format");
    }

    // --- UpdateUserPasswordRequest ---

    [Fact]
    public void UpdatePassword_ValidRequest_PassesValidation()
    {
        var request = new UpdateUserPasswordRequest { Id = 1, PasswordHash = "newpassword123" };
        var result = _updatePasswordValidator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void UpdatePassword_PasswordTooShort_FailsValidation()
    {
        var request = new UpdateUserPasswordRequest { Id = 1, PasswordHash = "short" };
        var result = _updatePasswordValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Password must be at least 8 characters");
    }
}
