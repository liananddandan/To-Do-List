namespace To_Do_List.JWT.Options;

public class JwtTokenOption
{
    public const string JwtKey = "JWT";
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string IssuerSigningKey { get; set; }
    public string AccessTokenExpiresMinutes { get; set; }
}