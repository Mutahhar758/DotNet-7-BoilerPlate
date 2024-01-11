namespace Demo.WebApi.Infrastructure.FileStorage;

public class AzureStorageSettings
{
    public string MediaAllowedExtension { get; set; } = default!;

    public string ConnectionString { get; set; } = default!;

    public string AzureContainerReference { get; set; } = default!;

    public string AzureBlobReference { get; set; } = default!;

    public string AccountName { get; set; } = default!;

    public string AccountKey { get; set; } = default!;

    public long FileSizeLimit { get; set; }
}