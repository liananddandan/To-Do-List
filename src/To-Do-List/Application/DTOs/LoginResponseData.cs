namespace To_Do_List.Application.DTOs;

public record LoginResponseData(ApiResponseCode Code, object? Info, 
    string? AccessToken, string? RefreshToken) : ResponseData(Code, Info);

