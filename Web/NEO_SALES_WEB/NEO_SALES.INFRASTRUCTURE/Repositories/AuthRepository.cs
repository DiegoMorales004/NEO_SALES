using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Models.Dtos.Auth;
using NEO_SALES.INFRASTRUCTURE.Http;

namespace NEO_SALES.INFRASTRUCTURE.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly IApiClient _apiClient;
    private readonly ApiConfiguration _apiConfiguration;

    public AuthRepository(IApiClient apiClient, IOptions<ApiConfiguration> apiConfiguration)
    {
        _apiClient = apiClient;
        _apiConfiguration = apiConfiguration.Value;
    }

    public Task<LoginResponseDto?> LoginAsync(LoginRequestDto request) =>
        _apiClient.PostAsync<LoginResponseDto>(_apiConfiguration.Endpoints.Login, request);
}
