using System.Text.Json.Serialization;

namespace Demo.WebApi.Application.Common.Models;
public class DataResponseDetail<T> : HttpResponseDetail
{
    [JsonExtensionData]
    public Dictionary<string, object> Data { get; set; }
}

public static class DataResponseExtesion
{
    public static DataResponseDetail<T> ToDataResponse<T>(this T data, string propName = "data")
    {
        return new DataResponseDetail<T> { Data = new Dictionary<string, object> { { propName, data } } };
    }
}