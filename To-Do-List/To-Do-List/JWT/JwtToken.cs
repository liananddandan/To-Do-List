using Microsoft.AspNetCore.Identity;

namespace SelfLearnProject.Entities;

public class JwtToken
{
    public string TokenStr { get; set; }
    public DateTime Expires  { get; set; }
}