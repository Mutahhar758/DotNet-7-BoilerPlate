using Microsoft.AspNetCore.Http;

namespace Demo.WebApi.Application.Public.Media;
public class MediaRequest
{
    public IFormFile MediaFile { get; set; } = default!;
    public string? Path { get; set; }
}
