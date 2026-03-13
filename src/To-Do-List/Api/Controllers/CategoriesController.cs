using Microsoft.AspNetCore.Mvc;
using To_Do_List.Application.DTOs;
using To_Do_List.Application.Services.Interface;
using To_Do_List.domain.Entities;

namespace To_Do_List.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(ITaskCategoryService taskCategoryService) : ProjectBaseController
{
    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var (code, categories) = await taskCategoryService.GetAllCategoryWithoutTasksAsync(UserId);
        if (code != ApiResponseCode.CategoryGetAllWithoutTasksSuccess || categories is null)
        {
            return BadRequest(new ResponseData(code, "Get all categories failed"));
        }

        return Ok(new ResponseData(code, categories));
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(CreateCategoryRequest request)
    {
        var (result, category) = await taskCategoryService.CreateCategoryAsync(request.Name, request.Description, UserId);
        if (result != ApiResponseCode.CategoryCreateSuccess || category == null)
        {
            return BadRequest(new ResponseData(result, null));
        }

        return Ok(new ResponseData(result, new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt,
            IsDefault = category.IsDefault,
            IsDeleted = category.IsDeleted,
            UserId = category.UserId
        }));
    }

    [HttpPut("{categoryId}")]
    public async Task<ActionResult> UpdateAsync(string categoryId, UpdateCategoryBodyRequest request)
    {
        var result = await taskCategoryService.UpdateCategoryAsync(categoryId, request.Name, request.Description, null, UserId);
        if (result != ApiResponseCode.CategoryUpdateSuccess)
        {
            return BadRequest(new ResponseData(result, "Category update failed"));
        }

        return Ok(new ResponseData(result, "Category updated successfully"));
    }

    [HttpDelete("{categoryId}")]
    public async Task<ActionResult> DeleteAsync(string categoryId)
    {
        var result = await taskCategoryService.DeleteCategoryById(categoryId, UserId);
        if (result != ApiResponseCode.DeleteCategorySuccess)
        {
            return BadRequest(new ResponseData(result, "Category delete failed"));
        }

        return Ok(new ResponseData(result, "Category deleted successfully"));
    }
}
