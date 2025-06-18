using FluentValidation;

namespace To_Do_List.Identity.Controllers.Login;

public record RegisterRequest(string UserName, string Password, string ConfirmPassword, string Email);

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.UserName).NotEmpty().NotNull();
        RuleFor(r => r.Password).NotEmpty().NotNull();
        RuleFor(r => r.ConfirmPassword).NotEmpty().NotNull().Equal(r => r.Password);
        RuleFor(r => r.Email).NotEmpty().NotNull().EmailAddress();
    }
}