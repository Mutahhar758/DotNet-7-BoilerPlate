namespace Demo.WebApi.Application.Public.Media;
public class MediaFromStreamRequest
{
    public byte[] Media { get; set; } = default!;
    public string MediaName { get; set; } = default!;
    public string Extension { get; set; } = default!;
    public string? Path { get; set; }
}
