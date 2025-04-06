namespace To_Do_List.Require;

public enum ApiResponseCode
{
    Success = 0,
    Failed = 1,
    ExceptionNotHandled = 2,
    ParameterError = 100001,
    UserCreateSuccess = 200000,
    UserLoginSuccess = 200001,
    UserExisted = 200101,
    UserCreatFailed = 200102,
    UserNotFound = 200103,
    UserPasswordError = 200104,
}