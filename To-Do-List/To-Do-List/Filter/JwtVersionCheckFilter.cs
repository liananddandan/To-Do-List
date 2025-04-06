using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using To_Do_List.Attribute;
using To_Do_List.Identity.Entities;

namespace To_Do_List.Filter;

public class JwtVersionCheckFilter(UserManager<MyUser> userManager) : IAsyncActionFilter
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

        if (user.Version > clientJwtVersion)
        {
            context.Result = new ObjectResult("JWTVersion expire"){StatusCode = 400};
            return;
        }
        await next();
    }
}