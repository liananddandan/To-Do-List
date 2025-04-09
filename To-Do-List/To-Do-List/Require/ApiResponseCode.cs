namespace To_Do_List.Require;

public enum ApiResponseCode
{
    Success = 0,
    Failed = 1,
    ExceptionNotHandled = 2,
    ParameterError = 100001,
    TokenPhraseUserIdError = 100002,
    UserCreateSuccess = 200000,
    UserLoginSuccess = 200001,
    UserChangePasswordSuccess = 200002,
    UserExisted = 200101,
    UserCreatFailed = 200102,
    UserNotFound = 200103,
    UserPasswordError = 200104,
    UserChangePasswordFailed = 200105,
    TaskCreateSuccess = 300000,
    TaskGetAllSuccess = 300001,
    CategoryCreateSuccess = 300002,
    CategoryUpdateSuccess = 300003,
    CategoryIdNotFoundInCurrentUser = 300101,
    TaskGetAllFailed = 300102,
}