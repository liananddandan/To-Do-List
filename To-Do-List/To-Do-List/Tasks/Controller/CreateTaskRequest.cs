using FluentValidation;

namespace To_Do_List.Tasks.Controller;

public record CreateTaskRequest(string Title, string? Description, DateTime? DueDate, int Priority, string? CategoryId);

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Priority).GreaterThan(0);
    }
}