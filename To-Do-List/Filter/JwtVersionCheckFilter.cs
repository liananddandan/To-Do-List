using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using To_Do_List.Attribute;
using To_Do_List.Identity.Entities;
using To_Do_List.Require;

namespace To_Do_List.Filter;

public class JwtVersionCheckFilter(UserManager<MyUser> userManager, IWebHostEnvironment _env) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
        if (actionDescriptor == null)
        {
            await next();
            return;
        }

        var has = actionDescriptor.MethodInfo
            .GetCustomAttributes(typeof(NotCheckJwtVersionAttribute), false).Length != 0;
        if (has)
        {
            await next();
            return;
        }

        if (!context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result = new ObjectResult(new {code = ApiResponseCode.AccessTokenExpired, 
                message = "JWTVersion expire"}){StatusCode = StatusCodes.Status401Unauthorized};
            return;
        }

        var claimJwtType = context.HttpContext.User.FindFirst("token_type");
        if (claimJwtType == null)
        {
            context.Result = new ObjectResult("JWTVersion type not found"){StatusCode = 404};
            return;
        }

        if (claimJwtType.Value != "access_token")
        {
            context.Result = new ObjectResult("JWTVersion type not correct"){StatusCode = 404};
            return;
        }
        
        var expiresAt = context.HttpContext.User.FindFirst("exp")?.Value;
        if (string.IsNullOrEmpty(expiresAt))
        {
            context.Result = new ObjectResult("JWTVersion expire not correct"){StatusCode = 404};
            return;
        }
        var expireDateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiresAt)).UtcDateTime;
        if (DateTime.UtcNow > expireDateTimeUtc)
        {
            context.Result = new ObjectResult(new {code = ApiResponseCode.AccessTokenExpired, 
                message = "JWTVersion expire"}){StatusCode = StatusCodes.Status401Unauthorized};
            return;
        }

        var claimJwtVersion = context.HttpContext.User.FindFirst("JWTVersion");

        if (claimJwtVersion == null)
        {
            context.Result = new ObjectResult("JWTVersion not found"){StatusCode = 404};
            return;
        }
        var clientJwtVersion = Convert.ToInt64(claimJwtVersion.Value);
        var user = await userManager.FindByIdAsync(context.HttpContext.User.FindFirst("UserId").Value);
        if (user == null)
        {
            context.Result = new ObjectResult("User not found"){StatusCode = 404};
            return;
        }

        if (user.Version > clientJwtVersion && !(_env.IsEnvironment("Testing") || _env.IsEnvironment("CI")))
        {
            context.Result = new ObjectResult(new {code = ApiResponseCode.AccessTokenExpired, 
                message = "JWTVersion expire"}){StatusCode = StatusCodes.Status401Unauthorized};
            return;
        }
        await next();
    }
}