using FluentValidation;

namespace To_Do_List.Application.DTOs;

public record UpdateCategoryBodyRequest(string? Name, string? Description);

public class UpdateCategoryBodyRequestValidator : AbstractValidator<UpdateCategoryBodyRequest>
{
    public UpdateCategoryBodyRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Name) || !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("At least one of Name or Description must be provided");
    }
}
