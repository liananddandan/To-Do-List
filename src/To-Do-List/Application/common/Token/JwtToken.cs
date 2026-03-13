namespace To_Do_List.Application.Common.Token;

public class JwtToken
{
    public string TokenStr { get; set; }
    public DateTime Expires  { get; set; }
}