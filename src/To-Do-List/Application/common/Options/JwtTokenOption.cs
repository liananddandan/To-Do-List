namespace To_Do_List.Application.Common.Options;

public class JwtTokenOption
{
    public const string JwtKey = "JWT";
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string IssuerSigningKey { get; set; }
    public int AccessTokenExpiresMinutes { get; set; }
    
    public int RefreshTokenExpiresDays { get; set; }
}