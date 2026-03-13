namespace To_Do_List.Application.DTOs;

public class ApiResponse<T>
{
    public ApiResponseCode Code { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
}