using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Models.Dtos;
using NEO_SALES.CORE.Models.Dtos.Sale;
using NEO_SALES.INFRASTRUCTURE.Http;

namespace NEO_SALES.INFRASTRUCTURE.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly IApiClient _apiClient;
    private readonly ApiConfiguration _apiConfiguration;

    public SaleRepository(IApiClient apiClient, IOptions<ApiConfiguration> apiConfiguration)
    {
        _apiClient = apiClient;
        _apiConfiguration = apiConfiguration.Value;
    }

    private string BasePath => _apiConfiguration.Endpoints.Sale;

    public async Task<List<SaleDto>> GetAllAsync() =>
        await _apiClient.GetAsync<List<SaleDto>>(BasePath) ?? [];

    public Task<SaleDto?> GetByIdAsync(Guid id) =>
        _apiClient.GetAsync<SaleDto>($"{BasePath}/{id}");

    public async Task<List<SaleDto>> GetByCustomerIdAsync(Guid customerId) =>
        await _apiClient.GetAsync<List<SaleDto>>($"{BasePath}/customer/{customerId}") ?? [];

    public Task<ShoppingCartDto?> GetShoppingCartByCustomerAsync(Guid customerId) =>
        _apiClient.GetAsync<ShoppingCartDto>($"{BasePath}/cart/{customerId}");

    public Task<SaleDto?> CreateAsync(SaleCreateDto dto) =>
        _apiClient.PostAsync<SaleDto>(BasePath, dto);

    public Task<ConfirmSaleResultDto?> ConfirmAsync(Guid id) =>
        _apiClient.PostAllowBadRequestAsync<ConfirmSaleResultDto>($"{BasePath}/confirm/{id}");
}
