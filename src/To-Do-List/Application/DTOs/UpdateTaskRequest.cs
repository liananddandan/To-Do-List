using FluentValidation;

namespace To_Do_List.Application.DTOs;

public record UpdateTaskRequest(
    string Title,
    string? Description,
    DateTime? DueDate,
    int Priority,
    string CategoryId,
    bool IsCompleted);

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Priority).GreaterThan(0);
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}
