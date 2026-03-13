using FluentValidation;

namespace To_Do_List.Application.DTOs;

public record DeleteCategoryRequest(string CategoryId);

public class DeleteCategoryRequestValidator : AbstractValidator<DeleteCategoryRequest>
{
    public DeleteCategoryRequestValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}