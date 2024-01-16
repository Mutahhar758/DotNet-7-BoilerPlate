using Newtonsoft.Json;

namespace NotificationFunction.Models;
public class FCMSettings
{
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "project_id")]
    public string ProjectId { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "private_key_id")]
    public string PrivatekeyId { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "private_key")]
    public string PrivateKey { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "client_email")]
    public string ClientEmail { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "client_id")]
    public string ClientId { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "auth_uri")]
    public string AuthUri { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "token_uri")]
    public string TokenUri { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "auth_provider_x509_cert_url")]
    public string AuthProviderx509CertUrl { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "client_x509_cert_url")]
    public string Clientx509CertUrl { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "universe_domain")]
    public string UniverseDomain { get; set; } = string.Empty;

}
