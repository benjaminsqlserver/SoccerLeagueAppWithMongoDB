using System.Net.Http.Json;

namespace SoccerLeague.Client.Shared.Services;

/// <summary>
/// Authentication service for WebAssembly - uses BFF pattern
/// </summary>
public class BffAuthenticationService
{
    private readonly HttpClient _httpClient;
    private UserInfo? _currentUser;

    public BffAuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public event Action? OnAuthenticationStateChanged;

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/bff/login", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserInfo>>();

                if (result?.Success == true && result.Data != null)
                {
                    _currentUser = result.Data;
                    OnAuthenticationStateChanged?.Invoke();

                    return new AuthResult { Success = true, Message = "Login successful" };
                }
            }

            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<UserInfo>>();
            return new AuthResult { Success = false, Message = errorResult?.Message ?? "Login failed" };
        }
        catch (Exception ex)
        {
            return new AuthResult { Success = false, Message = $"An error occurred: {ex.Message}" };
        }
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/bff/register", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserInfo>>();

                if (result?.Success == true && result.Data != null)
                {
                    _currentUser = result.Data;
                    OnAuthenticationStateChanged?.Invoke();

                    return new AuthResult { Success = true, Message = result.Message ?? "Registration successful" };
                }
            }

            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<UserInfo>>();
            return new AuthResult { Success = false, Message = errorResult?.Message ?? "Registration failed" };
        }
        catch (Exception ex)
        {
            return new AuthResult { Success = false, Message = $"An error occurred: {ex.Message}" };
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _httpClient.PostAsync("/api/bff/logout", null);
        }
        catch { }
        finally
        {
            _currentUser = null;
            OnAuthenticationStateChanged?.Invoke();
        }
    }

    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        if (_currentUser != null)
            return _currentUser;

        try
        {
            var response = await _httpClient.GetAsync("/api/bff/me");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserInfo>>();
                if (result?.Success == true && result.Data != null)
                {
                    _currentUser = result.Data;
                    return _currentUser;
                }
            }
        }
        catch { }

        return null;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var user = await GetCurrentUserAsync();
        return user != null;
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var response = await _httpClient.PostAsync("/api/bff/refresh", null);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Makes an authenticated API call through the BFF proxy
    /// </summary>
    public async Task<HttpResponseMessage> ProxyApiCallAsync(string path, HttpMethod method, object? body = null)
    {
        var request = new HttpRequestMessage(method, $"/api/bff/proxy/{path}");

        if (body != null)
        {
            request.Content = JsonContent.Create(body);
        }

        return await _httpClient.SendAsync(request);
    }

    /// <summary>
    /// Generic GET request through BFF
    /// </summary>
    public async Task<T?> GetAsync<T>(string path)
    {
        var response = await ProxyApiCallAsync(path, HttpMethod.Get);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        return default;
    }

    /// <summary>
    /// Generic POST request through BFF
    /// </summary>
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string path, TRequest data)
    {
        var response = await ProxyApiCallAsync(path, HttpMethod.Post, data);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        return default;
    }

    /// <summary>
    /// Generic PUT request through BFF
    /// </summary>
    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string path, TRequest data)
    {
        var response = await ProxyApiCallAsync(path, HttpMethod.Put, data);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        return default;
    }

    /// <summary>
    /// Generic DELETE request through BFF
    /// </summary>
    public async Task<bool> DeleteAsync(string path)
    {
        var response = await ProxyApiCallAsync(path, HttpMethod.Delete);
        return response.IsSuccessStatusCode;
    }
}


