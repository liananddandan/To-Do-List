namespace To_Do_List.Tasks.Entities;

public class CategoryDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDefault { get; set; }
    public string UserId { get; set; }
    public bool IsDeleted { get; set; }
    public List<TaskItemDto> Tasks { get; set; } = new();
    
    public Category ToEntity()
    {
        return new Category
        {
            Id = this.Id,
            Name = this.Name,
            Description = this.Description,
            CreatedAt = this.CreatedAt,
            UserId =this.UserId,
            IsDefault = this.IsDefault,
            Tasks = new List<TaskItem>()
        };
    }
}