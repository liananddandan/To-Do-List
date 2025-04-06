namespace To_Do_List.Require;

public record LoginResponseData(ApiResponseCode Code, object? Error, string? Token) : ResponseData(Code, Error);

