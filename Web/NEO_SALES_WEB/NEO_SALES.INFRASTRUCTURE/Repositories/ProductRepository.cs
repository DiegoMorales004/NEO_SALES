using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Models.Dtos;
using NEO_SALES.INFRASTRUCTURE.Http;

namespace NEO_SALES.INFRASTRUCTURE.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IApiClient _apiClient;
    private readonly ApiConfiguration _apiConfiguration;

    public ProductRepository(IApiClient apiClient, IOptions<ApiConfiguration> apiConfiguration)
    {
        _apiClient = apiClient;
        _apiConfiguration = apiConfiguration.Value;
    }

    private string BasePath => _apiConfiguration.Endpoints.Product;

    public async Task<List<ProductDto>> GetAllAsync() =>
        await _apiClient.GetAsync<List<ProductDto>>(BasePath) ?? [];

    public Task<ProductDto?> GetByIdAsync(Guid id) =>
        _apiClient.GetAsync<ProductDto>($"{BasePath}/{id}");

    public async Task<List<ProductDto>> SearchAsync(string term) =>
        await _apiClient.GetAsync<List<ProductDto>>($"{BasePath}/search/{Uri.EscapeDataString(term)}") ?? [];

    public Task<ProductDto?> CreateAsync(ProductCreateDto dto) =>
        _apiClient.PostAsync<ProductDto>(BasePath, dto);

    public Task<ProductDto?> EditAsync(Guid id, ProductEditDto dto) =>
        _apiClient.PutAsync<ProductDto>($"{BasePath}/{id}", dto);
}
