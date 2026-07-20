namespace NEO_SALES.CORE.Models.Configuration;

public class ApiConfiguration
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public ApiCredentialsConfiguration DefaultCredentials { get; set; } = new();
    public ApiEndpointsConfiguration Endpoints { get; set; } = new();
}

public class ApiCredentialsConfiguration
{
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class ApiEndpointsConfiguration
{
    public string Login { get; set; } = "api/auth/login";
    public string Customer { get; set; } = "api/customer";
    public string Product { get; set; } = "api/product";
    public string Sale { get; set; } = "api/sale";
    public string SaleDetail { get; set; } = "api/sale-detail";
    public string SaleStatus { get; set; } = "api/sale-status";
}
