using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SoccerLeague.Client.Shared.Services;

/// <summary>
/// Unified authentication service that works for both MAUI (direct API) and Web (BFF)-BFF Means Backnd For Front End (Offers Best Security)
/// </summary>
public class AuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiBaseUrl;
    private readonly bool _useBff;
    private UserInfo? _currentUser;

    private string? _cachedToken;
    private string? _cachedRefreshToken;

    private const string TokenKey = "authToken";
    private const string RefreshTokenKey = "refreshToken";

    // Constructor for WebAssembly (BFF mode)
    public AuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _useBff = true; // Web always uses BFF
    }

    // Constructor for MAUI (Direct API mode)
    public AuthenticationService(HttpClient httpClient, string apiBaseUrl)
    {
        _httpClient = httpClient;
        _apiBaseUrl = apiBaseUrl;
        _useBff = false; // MAUI uses direct API calls
    }

    public event Action? OnAuthenticationStateChanged;

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        if (_useBff)
        {
            return await LoginViaBffAsync(request);
        }
        else
        {
            return await LoginDirectAsync(request);
        }
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        if (_useBff)
        {
            return await RegisterViaBffAsync(request);
        }
        else
        {
            return await RegisterDirectAsync(request);
        }
    }

    public async Task LogoutAsync()
    {
        if (_useBff)
        {
            await LogoutViaBffAsync();
        }
        else
        {
            await LogoutDirectAsync();
        }
    }

    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        if (_currentUser != null)
            return _currentUser;

        if (_useBff)
        {
            return await GetCurrentUserViaBffAsync();
        }
        else
        {
            return await GetCurrentUserDirectAsync();
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        if (_useBff)
        {
            var user = await GetCurrentUserAsync();
            return user != null;
        }
        else
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }
    }

    public async Task<bool> RefreshTokenAsync()
    {
        if (_useBff)
        {
            return await RefreshTokenViaBffAsync();
        }
        else
        {
            return await RefreshTokenDirectAsync();
        }
    }

    #region BFF Mode (WebAssembly)

    private async Task<AuthResult> LoginViaBffAsync(LoginRequest request)
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

    private async Task<AuthResult> RegisterViaBffAsync(RegisterRequest request)
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

    private async Task LogoutViaBffAsync()
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

    private async Task<UserInfo?> GetCurrentUserViaBffAsync()
    {
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

    private async Task<bool> RefreshTokenViaBffAsync()
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

    #endregion

    #region Direct API Mode (MAUI)

    private async Task<AuthResult> LoginDirectAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();

                if (result?.Success == true && result.Data != null)
                {
                    await StoreTokensAsync(result.Data.AccessToken, result.Data.RefreshToken);
                    _currentUser = new UserInfo
                    {
                        UserId = result.Data.UserId,
                        Email = result.Data.Email,
                        FullName = result.Data.FullName,
                        Roles = result.Data.Roles
                    };

                    OnAuthenticationStateChanged?.Invoke();

                    return new AuthResult { Success = true, Message = "Login successful" };
                }
            }

            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
            return new AuthResult { Success = false, Message = errorResult?.Message ?? "Login failed" };
        }
        catch (Exception ex)
        {
            return new AuthResult { Success = false, Message = $"An error occurred: {ex.Message}" };
        }
    }

    private async Task<AuthResult> RegisterDirectAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Auth/register", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();

                if (result?.Success == true && result.Data != null)
                {
                    await StoreTokensAsync(result.Data.AccessToken, result.Data.RefreshToken);
                    _currentUser = new UserInfo
                    {
                        UserId = result.Data.UserId,
                        Email = result.Data.Email,
                        FullName = result.Data.FullName,
                        Roles = result.Data.Roles
                    };

                    OnAuthenticationStateChanged?.Invoke();

                    return new AuthResult { Success = true, Message = result.Message ?? "Registration successful" };
                }
            }

            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
            return new AuthResult { Success = false, Message = errorResult?.Message ?? "Registration failed" };
        }
        catch (Exception ex)
        {
            return new AuthResult { Success = false, Message = $"An error occurred: {ex.Message}" };
        }
    }

    private async Task LogoutDirectAsync()
    {
        try
        {
            var token = await GetTokenAsync();
            var refreshToken = await GetRefreshTokenAsync();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Auth/logout",
                    new { RefreshToken = refreshToken });
            }
        }
        catch { }
        finally
        {
            await ClearTokensAsync();
            _currentUser = null;
            OnAuthenticationStateChanged?.Invoke();
        }
    }

    private async Task<UserInfo?> GetCurrentUserDirectAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return null;

        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Auth/me");

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

    private async Task<bool> RefreshTokenDirectAsync()
    {
        try
        {
            var refreshToken = await GetRefreshTokenAsync();
            if (string.IsNullOrEmpty(refreshToken))
                return false;

            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Auth/refresh-token",
                new { RefreshToken = refreshToken });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();

                if (result?.Success == true && result.Data != null)
                {
                    await StoreTokensAsync(result.Data.AccessToken, result.Data.RefreshToken);
                    return true;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Token Storage (MAUI Only)

    private async Task StoreTokensAsync(string token, string refreshToken)
    {
#if ANDROID || IOS || MACCATALYST || WINDOWS
        await SecureStorage.SetAsync(TokenKey, token);
        await SecureStorage.SetAsync(RefreshTokenKey, refreshToken);
#endif
        _cachedToken = token;
        _cachedRefreshToken = refreshToken;
    }

    private async Task<string?> GetTokenAsync()
    {
#if ANDROID || IOS || MACCATALYST || WINDOWS
        _cachedToken ??= await SecureStorage.GetAsync(TokenKey);
#endif
        return _cachedToken;
    }

    private async Task<string?> GetRefreshTokenAsync()
    {
#if ANDROID || IOS || MACCATALYST || WINDOWS
        _cachedRefreshToken ??= await SecureStorage.GetAsync(RefreshTokenKey);
#endif
        return _cachedRefreshToken;
    }

    private async Task ClearTokensAsync()
    {
#if ANDROID || IOS || MACCATALYST || WINDOWS
        SecureStorage.Remove(TokenKey);
        SecureStorage.Remove(RefreshTokenKey);
#endif
        _cachedToken = null;
        _cachedRefreshToken = null;
        await Task.CompletedTask;
    }

    public void SetAuthorizationHeader(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    #endregion

    #region Helper Methods for API Calls (can be used by both modes)

    /// <summary>
    /// Makes an authenticated API call - automatically routes through BFF if in web mode
    /// </summary>
    public async Task<HttpResponseMessage> MakeAuthenticatedCallAsync(string path, HttpMethod method, object? body = null)
    {
        if (_useBff)
        {
            var request = new HttpRequestMessage(method, $"/api/bff/proxy/{path}");

            if (body != null)
            {
                request.Content = JsonContent.Create(body);
            }

            return await _httpClient.SendAsync(request);
        }
        else
        {
            var token = await GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var request = new HttpRequestMessage(method, $"{_apiBaseUrl}/{path}");

            if (body != null)
            {
                request.Content = JsonContent.Create(body);
            }

            return await _httpClient.SendAsync(request);
        }
    }

    #endregion
}

// DTOs
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}

public class AuthResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime TokenExpiry { get; set; }
    public bool EmailConfirmed { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}

public class UserInfo
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string FullName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public bool EmailConfirmed { get; set; }
}

public class AuthResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}