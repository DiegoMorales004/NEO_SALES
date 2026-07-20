using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Models.Dtos;
using NEO_SALES.INFRASTRUCTURE.Http;

namespace NEO_SALES.INFRASTRUCTURE.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IApiClient _apiClient;
    private readonly ApiConfiguration _apiConfiguration;

    public CustomerRepository(IApiClient apiClient, IOptions<ApiConfiguration> apiConfiguration)
    {
        _apiClient = apiClient;
        _apiConfiguration = apiConfiguration.Value;
    }

    private string BasePath => _apiConfiguration.Endpoints.Customer;

    public async Task<List<CustomerDto>> GetAllAsync() =>
        await _apiClient.GetAsync<List<CustomerDto>>(BasePath) ?? [];

    public Task<CustomerDto?> GetByIdAsync(Guid id) =>
        _apiClient.GetAsync<CustomerDto>($"{BasePath}/{id}");

    public async Task<List<CustomerDto>> SearchAsync(string term) =>
        await _apiClient.GetAsync<List<CustomerDto>>($"{BasePath}/search/{Uri.EscapeDataString(term)}") ?? [];

    public Task<CustomerDto?> CreateAsync(CustomerCreateDto dto) =>
        _apiClient.PostAsync<CustomerDto>(BasePath, dto);

    public Task<CustomerDto?> EditAsync(Guid id, CustomerEditDto dto) =>
        _apiClient.PutAsync<CustomerDto>($"{BasePath}/{id}", dto);
}
