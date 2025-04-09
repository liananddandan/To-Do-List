using Microsoft.AspNetCore.Mvc;
using To_Do_List.Controller;
using To_Do_List.Require;
using To_Do_List.Tasks.Service;

namespace To_Do_List.Tasks.Controller;

[ApiController]
[Route("[controller]/[action]")]
public class TaskController(
    TaskCategoryService taskCategoryService) : ProjectBaseController
{
    [HttpPost]
    public async Task<ActionResult> CreateTaskAsync(CreateTaskRequest request)
    {
        var result = await taskCategoryService.CreateTaskAsync(request.Title, request.Description, request.DueDate,
            request.Priority, UserId, request.CategoryId);
        if (result == ApiResponseCode.TaskCreateSuccess)
        {
            return Ok(new ResponseData(result, "Task created successfully"));
        }
        else
        {
            return BadRequest(new ResponseData(result, "Task creation failed"));
        }
    }

    [HttpGet]
    public async Task<ActionResult> GetAllTasksAsync()
    {
        var result = await taskCategoryService.GetAllCategoryWithTasksAsync(UserId);
        return Ok(new ResponseData(ApiResponseCode.TaskGetAllSuccess, new {Categories = result}));
    }

    [HttpPost]
    public async Task<ActionResult> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var result = await taskCategoryService.CreateCategoryAsync(request.Name, request.Description, UserId);
        if (result == ApiResponseCode.TaskCreateSuccess)
        {
            return Ok(new ResponseData(result, "Category created successfully"));
        }
        else
        {
            return BadRequest(new ResponseData(result, "Category creation failed"));
        }
    }

    [HttpPut]
    public async Task<ActionResult> UpdateCategoryAsync(UpdateCategoryRequest request)
    {
        var result = await taskCategoryService.UpdateCategoryAsync(request.CategoryId, request.Name, request.Description, UserId);
        if (result != ApiResponseCode.CategoryUpdateSuccess)
        {
            return BadRequest(new ResponseData(result, "Category update failed"));
        }
        else
        {
            return Ok(new ResponseData(result, "Category updated successfully"));
        }
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteCategoryAsync(DeleteCategoryRequest request)
    {
        var result = await taskCategoryService.DeleteCategoryById(request.CategoryId, UserId);
        if (result != ApiResponseCode.DeleteCategorySuccess)
        {
            return BadRequest(new ResponseData(result, "Category delete failed"));
        }
        else
        {
            return Ok(new ResponseData(result, "Category deleted successfully"));
        }
    }
}