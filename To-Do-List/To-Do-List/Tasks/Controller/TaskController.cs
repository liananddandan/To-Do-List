using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using To_Do_List.Require;
using To_Do_List.Tasks.Service;

namespace To_Do_List.Tasks.Controller;

[ApiController]
[Route("[controller]/[action]")]
public class TaskController(TaskCategoryService taskCategoryService, 
   CreateTaskRequestValidator createTaskRequestValidator) : ControllerBase
{

   [HttpPost]
   public async Task<ActionResult> CreateTask(CreateTaskRequest request)
   {
      var validatorResult = await createTaskRequestValidator.ValidateAsync(request);
      if (!validatorResult.IsValid)
      {
         return BadRequest(new ResponseData(ApiResponseCode.ParameterError,validatorResult.Errors.Select(x => x.ErrorMessage).ToList()));
      }

      var userId = User.FindFirstValue("UserId");
      if (userId == null)
      {
         return BadRequest(new ResponseData(ApiResponseCode.UserNotFound, "User not found"));
      }

      var result = await taskCategoryService.CreateTaskAsync(request.Title, request.Description, request.DueDate,
         request.Priority, userId, request.CategoryId);
      if (result == ApiResponseCode.TaskCreateSuccess)
      {
         return Ok(new ResponseData(result, "Task created successfully"));
      }
      else
      {
         return BadRequest(new ResponseData(result, "Task creation failed"));
      }
   }
}