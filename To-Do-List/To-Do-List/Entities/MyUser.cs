using Microsoft.AspNetCore.Identity;

namespace To_Do_List.Entities;

public class MyUser : IdentityUser<long>
{
    public Guid UserGuid { get; set; }
}