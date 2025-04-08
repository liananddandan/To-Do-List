using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using To_Do_List.Tasks.Entities;

namespace To_Do_List.Tasks.Configs;

public class TaskItemConfig : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasQueryFilter(t => !t.IsDeleted);
        builder.HasOne<Category>(c => c.Category).WithMany(c => c.Tasks);
    }
}