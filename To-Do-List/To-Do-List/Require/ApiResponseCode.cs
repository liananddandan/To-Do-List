namespace To_Do_List.Require;

public enum ApiResponseCode
{
    Success = 0,
    Failed = 1,
    ExceptionNotHandled = 2,
    ParameterError = 10001,
    UserCreateSuccess = 20000,
    UserExisted = 20001,
    UserCreatFailed = 20002,
}