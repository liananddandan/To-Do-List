using FluentValidation;

namespace To_Do_List.Application.DTOs;

public record RefreshTokenRequest(string RefreshToken);

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}