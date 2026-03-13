using Microsoft.AspNetCore.Mvc;
using To_Do_List.Application.DTOs;
using To_Do_List.Application.Services.Interface;

namespace To_Do_List.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController(ITaskCategoryService taskCategoryService) : ProjectBaseController
{
    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var result = await taskCategoryService.GetAllCategoryWithTasksAsync(UserId);
        return Ok(new ResponseData(ApiResponseCode.TaskGetAllSuccess, result));
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(CreateTaskRequest request)
    {
        var result = await taskCategoryService.CreateTaskAsync(
            request.Title,
            request.Description,
            request.DueDate,
            request.Priority,
            UserId,
            request.CategoryId);

        if (result != ApiResponseCode.TaskCreateSuccess)
        {
            return BadRequest(new ResponseData(result, "Task creation failed"));
        }

        return Ok(new ResponseData(result, "Task created successfully"));
    }

    [HttpPut("{taskId}")]
    public async Task<ActionResult> UpdateAsync(string taskId, UpdateTaskRequest request)
    {
        var result = await taskCategoryService.UpdateTaskAsync(
            taskId,
            request.Title,
            request.Description,
            request.DueDate,
            request.Priority,
            UserId,
            request.CategoryId,
            request.IsCompleted);

        if (result != ApiResponseCode.TaskUpdateSuccess)
        {
            return BadRequest(new ResponseData(result, "Task update failed"));
        }

        return Ok(new ResponseData(result, "Task updated successfully"));
    }

    [HttpPatch("{taskId}/completion")]
    public async Task<ActionResult> UpdateCompletionAsync(string taskId, UpdateTaskCompletionRequest request)
    {
        var result = await taskCategoryService.UpdateTaskCompletionAsync(taskId, request.IsCompleted, UserId);
        if (result != ApiResponseCode.TaskCompletionUpdateSuccess)
        {
            return BadRequest(new ResponseData(result, "Task completion update failed"));
        }

        return Ok(new ResponseData(result, "Task completion updated successfully"));
    }

    [HttpDelete("{taskId}")]
    public async Task<ActionResult> DeleteAsync(string taskId)
    {
        var result = await taskCategoryService.DeleteTaskById(taskId, UserId);
        if (result != ApiResponseCode.TaskDeleteSuccess)
        {
            return BadRequest(new ResponseData(result, "Task delete failed"));
        }

        return Ok(new ResponseData(result, "Task deleted successfully"));
    }
}
