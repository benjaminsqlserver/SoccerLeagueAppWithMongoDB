using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace SoccerLeague.Client.Shared.Services;

public class AuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private const string TokenKey = "authToken";
    private const string RefreshTokenKey = "refreshToken";

    public AuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _baseUrl = "https://localhost:7176/api";
    }

    public event Action? OnAuthenticationStateChanged;

    private string? _cachedToken;
    private string? _cachedRefreshToken;
    private UserInfo? _currentUser;

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Auth/login", request);

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

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Auth/register", request);

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

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var refreshToken = await GetRefreshTokenAsync();
            if (string.IsNullOrEmpty(refreshToken))
                return false;

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Auth/refresh-token",
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

    public async Task LogoutAsync()
    {
        try
        {
            var token = await GetTokenAsync();
            var refreshToken = await GetRefreshTokenAsync();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                await _httpClient.PostAsJsonAsync($"{_baseUrl}/Auth/logout",
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

    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        if (_currentUser != null)
            return _currentUser;

        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return null;

        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_baseUrl}/Auth/me");

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
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    private async Task StoreTokensAsync(string token, string refreshToken)
    {
        _cachedToken = token;
        _cachedRefreshToken = refreshToken;
        // In a real app, you'd store these securely using platform-specific storage
        await Task.CompletedTask;
    }

    private async Task<string?> GetTokenAsync()
    {
        await Task.CompletedTask;
        return _cachedToken;
    }

    private async Task<string?> GetRefreshTokenAsync()
    {
        await Task.CompletedTask;
        return _cachedRefreshToken;
    }

    private async Task ClearTokensAsync()
    {
        _cachedToken = null;
        _cachedRefreshToken = null;
        await Task.CompletedTask;
    }

    public void SetAuthorizationHeader(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
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