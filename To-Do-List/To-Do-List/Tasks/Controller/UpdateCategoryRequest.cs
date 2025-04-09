using FluentValidation;

namespace To_Do_List.Tasks.Controller;

public record UpdateCategoryRequest(long CategoryId, string? Name, string? Description);

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Name)
            || !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Either Name or Description must be provided");
    }
}