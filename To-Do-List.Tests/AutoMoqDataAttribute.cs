using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using To_Do_List.Tasks.Entities;

namespace To_Do_List.Tests;

public class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute()
        : base(() =>
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Customize<Category>(
                c => c.Without(x => x.Tasks));
            return fixture;
        })
    {
    }
}