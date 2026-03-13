using Microsoft.AspNetCore.Identity;

namespace To_Do_List.domain.Entities;

public class MyUser : IdentityUser<long>
{
    public Guid UserGuid { get; set; }
    public long Version { get; set; }
}