using Microsoft.AspNetCore.Identity;

namespace To_Do_List.Identity.Entities;

public class MyUser : IdentityUser<long>
{
    public Guid UserGuid { get; set; }
    public long Version { get; set; }
}