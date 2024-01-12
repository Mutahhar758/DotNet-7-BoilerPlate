using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Demo.WebApi.Application.Public.Notifications;
public class BatchNotificationRequest
{
    public string Title { get; set; } = default!;
    public string Body { get; set; } = default!;
    public string? Url { get; set; }
    [DataType(DataType.Upload)]
    public IFormFile? Image { get; set; } = default!;
    public List<int>? CustomerIds { get; set; }
    public List<int>? DriverIds { get; set; }
    public List<int>? CorporateIds { get; set; }
    public bool AllCustomers { get; set; }
    public bool AllDrivers { get; set; }
    public bool AllCorporates { get; set; }
}
