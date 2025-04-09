using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using To_Do_List.Require;

namespace To_Do_List.Filter;

public class ValidationFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var arg in context.ActionArguments)
        {
            var argumentType = arg.Value?.GetType();
            if (argumentType == null) continue;
            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
            var validator = serviceProvider.GetService(validatorType);
            if (validator is IValidator val)
            {
                var validationResult = await val.ValidateAsync(new ValidationContext<object>(arg.Value!));
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                    context.Result = new BadRequestObjectResult(
                        new ResponseData(ApiResponseCode.ParameterError, errors));
                    return;
                }
            }
        }
        await next();
    }
}