namespace NEO_SALES.INFRASTRUCTURE.Http;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object? body = null);
    Task<T?> PutAsync<T>(string endpoint, object? body = null);
    Task DeleteAsync(string endpoint);
    Task<T?> PostAllowBadRequestAsync<T>(string endpoint, object? body = null);
}
