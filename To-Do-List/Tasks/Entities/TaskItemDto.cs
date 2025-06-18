namespace To_Do_List.Tasks.Entities;

public class TaskItemDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public int Priority { get; set; }
}