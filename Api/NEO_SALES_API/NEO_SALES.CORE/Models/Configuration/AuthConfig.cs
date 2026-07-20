namespace NEO_SALES.CORE.Models.Configuration;

public class AuthConfig
{
    public string Issuer { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; }
    public string SecretKey { get; set; } = string.Empty;
}
