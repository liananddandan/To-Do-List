using FluentValidation;

namespace To_Do_List.Application.DTOs;

public record UpdateTaskCompletionRequest(bool IsCompleted);

public class UpdateTaskCompletionRequestValidator : AbstractValidator<UpdateTaskCompletionRequest>
{
    public UpdateTaskCompletionRequestValidator()
    {
    }
}
