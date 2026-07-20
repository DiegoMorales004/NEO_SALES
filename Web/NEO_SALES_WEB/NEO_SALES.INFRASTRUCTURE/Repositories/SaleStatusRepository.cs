using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Models.Dtos;
using NEO_SALES.INFRASTRUCTURE.Http;

namespace NEO_SALES.INFRASTRUCTURE.Repositories;

public class SaleStatusRepository : ISaleStatusRepository
{
    private readonly IApiClient _apiClient;
    private readonly ApiConfiguration _apiConfiguration;

    public SaleStatusRepository(IApiClient apiClient, IOptions<ApiConfiguration> apiConfiguration)
    {
        _apiClient = apiClient;
        _apiConfiguration = apiConfiguration.Value;
    }

    private string BasePath => _apiConfiguration.Endpoints.SaleStatus;

    public async Task<List<SaleStatusDto>> GetAllAsync() =>
        await _apiClient.GetAsync<List<SaleStatusDto>>(BasePath) ?? [];

    public Task<SaleStatusDto?> GetByIdAsync(int id) =>
        _apiClient.GetAsync<SaleStatusDto>($"{BasePath}/{id}");
}
