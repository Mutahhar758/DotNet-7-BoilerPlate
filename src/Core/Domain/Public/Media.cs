namespace Demo.WebApi.Domain.Public;
public class Media : AuditableEntity, IAggregateRoot
{
    public string FileType { get; set; } = default!;

    public string ConvertedFileName { get; set; } = default!;

    public string OriginalFileName { get; set; } = default!;

    public string? Path { get; set; }

    public string? AccessURL { get; set; }
}
