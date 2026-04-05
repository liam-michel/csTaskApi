using FluentAssertions;
using TaskApi.Models.Dtos;
using TaskApi.Validators;

namespace TaskApi.Tests.Validators;

public class AuthValidatorTests
{
    private readonly LoginRequestValidator _loginValidator = new();
    private readonly RegisterRequestValidator _registerValidator = new();

    // --- LoginRequest ---

    [Fact]
    public void Login_ValidRequest_PassesValidation()
    {
        var request = new LoginRequest { Email = "user@example.com", Password = "password123" };
        var result = _loginValidator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Login_EmptyEmail_FailsValidation()
    {
        var request = new LoginRequest { Email = "", Password = "password123" };
        var result = _loginValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Login_InvalidEmailFormat_FailsValidation()
    {
        var request = new LoginRequest { Email = "not-an-email", Password = "password123" };
        var result = _loginValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Invalid email format");
    }

    [Fact]
    public void Login_EmptyPassword_FailsValidation()
    {
        var request = new LoginRequest { Email = "user@example.com", Password = "" };
        var result = _loginValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    // --- RegisterRequest ---

    [Fact]
    public void Register_ValidRequest_PassesValidation()
    {
        var request = new RegisterRequest { Username = "liam", Email = "liam@example.com", Password = "securepass1" };
        var result = _registerValidator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Register_EmptyUsername_FailsValidation()
    {
        var request = new RegisterRequest { Username = "", Email = "liam@example.com", Password = "securepass1" };
        var result = _registerValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Username");
    }

    [Fact]
    public void Register_UsernameTooLong_FailsValidation()
    {
        var request = new RegisterRequest { Username = new string('a', 51), Email = "liam@example.com", Password = "securepass1" };
        var result = _registerValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Username must be 1-50 characters");
    }

    [Fact]
    public void Register_PasswordTooShort_FailsValidation()
    {
        var request = new RegisterRequest { Username = "liam", Email = "liam@example.com", Password = "short" };
        var result = _registerValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Password must be at least 8 characters");
    }
}
