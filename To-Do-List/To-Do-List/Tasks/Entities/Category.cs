namespace To_Do_List.Tasks.Entities;

public class Category
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string UserId { get; set; }
    public bool IsDefault { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    
    public CategoryDto ToDto()
    {
        return new CategoryDto
        {
            Id = this.Id,
            Name = this.Name,
            Description = this.Description,
            CreatedAt = this.CreatedAt,
            IsDefault = this.IsDefault,
            UserId = this.UserId,
            IsDeleted = this.IsDeleted,
            Tasks = this.Tasks!
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.Priority)
                .Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    IsCompleted = t.IsCompleted,
                    CreatedDate = t.CreatedDate,
                    Priority = t.Priority
                }).ToList()
        };
    }
}