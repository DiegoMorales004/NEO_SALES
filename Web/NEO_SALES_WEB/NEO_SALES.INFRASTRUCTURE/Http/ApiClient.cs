using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Exceptions;
using NEO_SALES.CORE.Interfaces;
using NEO_SALES.CORE.Models.Configuration;

namespace NEO_SALES.INFRASTRUCTURE.Http;

public class ApiClient : IApiClient
{
    private const string HttpClientName = "NeoSalesApi";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAccessTokenProvider _tokenProvider;
    private readonly MessageDefaultsConfiguration _messages;

    public ApiClient(
        IHttpClientFactory httpClientFactory,
        IAccessTokenProvider tokenProvider,
        IOptions<MessageDefaultsConfiguration> messages)
    {
        _httpClientFactory = httpClientFactory;
        _tokenProvider = tokenProvider;
        _messages = messages.Value;
    }

    public Task<T?> GetAsync<T>(string endpoint) => SendAsync<T>(HttpMethod.Get, endpoint, null);

    public Task<T?> PostAsync<T>(string endpoint, object? body = null) => SendAsync<T>(HttpMethod.Post, endpoint, body);

    public Task<T?> PutAsync<T>(string endpoint, object? body = null) => SendAsync<T>(HttpMethod.Put, endpoint, body);

    public Task<T?> PostAllowBadRequestAsync<T>(string endpoint, object? body = null) =>
        SendAsync<T>(HttpMethod.Post, endpoint, body, treatBadRequestAsPayload: true);

    public async Task DeleteAsync(string endpoint)
    {
        await SendAsync<object>(HttpMethod.Delete, endpoint, null);
    }

    private async Task<T?> SendAsync<T>(HttpMethod method, string endpoint, object? body, bool treatBadRequestAsPayload = false)
    {
        var client = _httpClientFactory.CreateClient(HttpClientName);

        using var request = new HttpRequestMessage(method, endpoint);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        var token = _tokenProvider.GetToken();
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(request);
        }
        catch (TaskCanceledException ex)
        {
            throw new ApiCommunicationException(_messages.TimeOut, ex);
        }
        catch (HttpRequestException ex)
        {
            throw new ApiCommunicationException(_messages.InternalError, ex);
        }

        if (response.IsSuccessStatusCode || (treatBadRequestAsPayload && response.StatusCode == HttpStatusCode.BadRequest))
        {
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return default;
            }

            var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions);
        }

        var message = await ReadErrorMessageAsync(response);

        throw response.StatusCode switch
        {
            HttpStatusCode.BadRequest => new ApiBadRequestException(message),
            HttpStatusCode.Unauthorized => new ApiUnauthorizedException(message),
            HttpStatusCode.NotFound => new ApiNotFoundException(message),
            _ => new ApiCommunicationException(message)
        };
    }

    private async Task<string> ReadErrorMessageAsync(HttpResponseMessage response)
    {
        try
        {
            var payload = await response.Content.ReadFromJsonAsync<ApiErrorPayload>(JsonOptions);
            if (!string.IsNullOrWhiteSpace(payload?.Message))
            {
                return payload.Message;
            }
        }
        catch (JsonException)
        {
            
        }

        return _messages.InternalError;
    }

    private class ApiErrorPayload
    {
        public string? Message { get; set; }
    }
}
