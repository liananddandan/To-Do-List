namespace To_Do_List.domain.Entities;

public class TaskItem
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public int Priority { get; set; }
    public bool IsDeleted { get; set; } = false;
    public long CategoryId { get; set; }
    public Category Category { get; set; }

}
