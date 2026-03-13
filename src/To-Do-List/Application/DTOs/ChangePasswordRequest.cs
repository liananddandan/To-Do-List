using FluentValidation;

namespace To_Do_List.Application.DTOs;

public record ChangePasswordRequest(string Password, string ConfirmPassword);

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.Password).NotEmpty().NotNull();
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().NotNull().Equal(x => x.Password);
    }
}