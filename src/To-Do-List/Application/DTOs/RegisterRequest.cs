using FluentValidation;

namespace To_Do_List.Application.DTOs;

public record RegisterRequest(string UserName, string Password, string Email);

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.UserName)
            .NotEmpty();

        RuleFor(r => r.Password)
            .NotEmpty();

        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress();
    }
}