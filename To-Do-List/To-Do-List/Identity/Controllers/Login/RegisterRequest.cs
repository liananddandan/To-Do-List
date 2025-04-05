using FluentValidation;

namespace To_Do_List.Identity.Controllers.Login;

public record RegisterRequest(string userName, string password, string confirmPassword, string email);

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.userName).NotEmpty().NotNull();
        RuleFor(r => r.password).NotEmpty().NotNull();
        RuleFor(r => r.confirmPassword).NotEmpty().NotNull().Equal(r => r.password);
        RuleFor(r => r.email).NotEmpty().NotNull().EmailAddress();
    }
}