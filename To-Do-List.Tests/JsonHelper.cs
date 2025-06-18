using System.Text.Json;

namespace To_Do_List.Tests;

public static class JsonHelper
{
    // 如果去改后端的代码，让测试方便。那么后续的问题就是，前端所有的代码都要改，而且javascript也习惯用camel，而不是首字母大写
    private static readonly JsonSerializerOptions _camelCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };
    
    public static T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _camelCaseOptions);
}