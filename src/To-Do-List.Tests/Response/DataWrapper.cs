namespace To_Do_List.Tests.Response;

public class DataWrapper<T>
{
    public int Code { get; set; }
    public T Info { get; set; }
}