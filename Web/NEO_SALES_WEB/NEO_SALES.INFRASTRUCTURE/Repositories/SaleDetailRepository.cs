using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Models.Dtos;
using NEO_SALES.INFRASTRUCTURE.Http;

namespace NEO_SALES.INFRASTRUCTURE.Repositories;

public class SaleDetailRepository : ISaleDetailRepository
{
    private readonly IApiClient _apiClient;
    private readonly ApiConfiguration _apiConfiguration;

    public SaleDetailRepository(IApiClient apiClient, IOptions<ApiConfiguration> apiConfiguration)
    {
        _apiClient = apiClient;
        _apiConfiguration = apiConfiguration.Value;
    }

    private string BasePath => _apiConfiguration.Endpoints.SaleDetail;

    public async Task<List<SaleDetailDto>> GetAllAsync() =>
        await _apiClient.GetAsync<List<SaleDetailDto>>(BasePath) ?? [];

    public Task<SaleDetailDto?> GetByIdAsync(Guid id) =>
        _apiClient.GetAsync<SaleDetailDto>($"{BasePath}/{id}");

    public async Task<List<SaleDetailDto>> GetBySaleIdAsync(Guid saleId) =>
        await _apiClient.GetAsync<List<SaleDetailDto>>($"{BasePath}/sale/{saleId}") ?? [];

    public Task<SaleDetailDto?> CreateAsync(SaleDetailCreateDto dto) =>
        _apiClient.PostAsync<SaleDetailDto>(BasePath, dto);

    public Task<SaleDetailDto?> EditAsync(Guid id, SaleDetailEditDto dto) =>
        _apiClient.PutAsync<SaleDetailDto>($"{BasePath}/{id}", dto);

    public Task DeleteAsync(Guid id) =>
        _apiClient.DeleteAsync($"{BasePath}/{id}");
}
