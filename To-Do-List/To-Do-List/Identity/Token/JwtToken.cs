using Microsoft.AspNetCore.Identity;

namespace To_Do_List.Identity.Token;

public class JwtToken
{
    public string TokenStr { get; set; }
    public DateTime Expires  { get; set; }
}