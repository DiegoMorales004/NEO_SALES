using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Interfaces;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Services;
using NEO_SALES.INFRASTRUCTURE.Auth;
using NEO_SALES.INFRASTRUCTURE.Http;
using NEO_SALES.INFRASTRUCTURE.Repositories;

namespace NEO_SALES.INFRASTRUCTURE.Extensions;

public static class ProgramExtensions
{

    public static IServiceCollection AddNeoSalesWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiConfiguration>(configuration.GetSection("ApiConfiguration"));
        services.Configure<MessageDefaultsConfiguration>(configuration.GetSection("MessageDefaultsConfiguration"));

        // Infrastructure
        services.AddHttpContextAccessor();

        services.AddHttpClient("NeoSalesApi", (sp, client) =>
        {
            var apiConfiguration = sp.GetRequiredService<IOptions<ApiConfiguration>>().Value;
            client.BaseAddress = new Uri(apiConfiguration.Url);
        });

        services.AddScoped<IAccessTokenProvider, AccessTokenProvider>();
        services.AddScoped<IApiClient, ApiClient>();

        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<ISaleDetailRepository, SaleDetailRepository>();
        services.AddScoped<ISaleStatusRepository, SaleStatusRepository>();

        // Core
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<ISaleDetailService, SaleDetailService>();
        services.AddScoped<ISaleStatusService, SaleStatusService>();

        return services;
    }
}
