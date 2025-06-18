namespace To_Do_List.Require;

public record LoginResponseData(ApiResponseCode Code, object? Info, 
    string? AccessToken, string? RefreshToken) : ResponseData(Code, Info);

