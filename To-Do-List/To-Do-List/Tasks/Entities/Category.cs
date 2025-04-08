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
}