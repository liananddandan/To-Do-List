using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using To_Do_List.Require;

namespace To_Do_List.Filter;

public class ApiResponseWrapperFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception != null)
        {
            context.Result = new ObjectResult(new ApiResponse<string>()
            {
                Code = ApiResponseCode.ExceptionNotHandled,
                Message = context.Exception.Message,
                Data = context.Exception.StackTrace
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
            return;
        }

        switch (context.Result)
        {
            case ObjectResult objectResult:
                if (objectResult.Value is ApiResponse<object>) return;

                var wrapped = new ApiResponse<object>
                {
                    Code = (objectResult.StatusCode ?? 200) >= 400 ? ApiResponseCode.Failed : ApiResponseCode.Success,
                    Message = (objectResult.StatusCode ?? 200) >= 400 ? "Failed" : "Success",
                    Data = objectResult.Value
                };
                context.Result = new ObjectResult(wrapped)
                {
                    StatusCode = objectResult.StatusCode
                };
                break;
            case EmptyResult:
                context.Result = new ObjectResult(new ApiResponse<string>()
                {
                    Code = ApiResponseCode.Success,
                    Message = "Success",    
                    Data = null
                });
                break;
            case StatusCodeResult statusResult:
                context.Result = new ObjectResult(new ApiResponse<string>()
                {
                    Code = statusResult.StatusCode >= 400 ? ApiResponseCode.Failed : ApiResponseCode.Success,
                    Message = statusResult.StatusCode >= 400 ? "Failed" : "Success",
                    Data = null
                })
                {
                    StatusCode = statusResult.StatusCode
                };
                break;
        }
    }
}