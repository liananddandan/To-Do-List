using FluentValidation;

namespace To_Do_List.Tasks.Controller;

public record UpdateCategoryRequest(string CategoryId, string? Name, string? Description, Boolean? isDeleted);

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Name)
            || !string.IsNullOrWhiteSpace(x.Description)
            || x.isDeleted.HasValue)
            .WithMessage("At least one of Name, Description or isDeleted must be provided");
    }
}