using FluentValidation;

namespace To_Do_List.Application.DTOs;

public record CreateCategoryRequest(string Name, string? Description);

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
