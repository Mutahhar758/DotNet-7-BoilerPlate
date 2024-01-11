using Demo.WebApi.Application.Common.Enums;

namespace Demo.WebApi.Application.Common.Models;
public class HttpResponseDto
{
    public string Type { get; set; } = HttpResponseType.Information.ToString();
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public object? ValidationErrors { get; set; }
}

public class HttpResponseDetail
{
    public HttpResponseDto Message { get; set; }
}
