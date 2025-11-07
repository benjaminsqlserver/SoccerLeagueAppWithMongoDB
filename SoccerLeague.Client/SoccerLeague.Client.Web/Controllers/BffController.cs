using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SoccerLeague.Client.Web.Controllers;

[ApiController]
[Route("api/bff")]
public class BffController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private const string AuthCookieName = "SoccerLeague.Auth";
    private const string RefreshCookieName = "SoccerLeague.Refresh";

    public BffController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var client = _httpClientFactory.CreateClient();
        var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7176/api";

        try
        {
            var response = await client.PostAsJsonAsync($"{apiBaseUrl}/Auth/login", request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<AuthResponse>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result?.Success == true && result.Data != null)
                {
                    // Store tokens in HTTP-only cookies
                    SetAuthCookies(result.Data.AccessToken, result.Data.RefreshToken, request.RememberMe);

                    // Return user info without tokens
                    return Ok(new ApiResponse<UserInfo>
                    {
                        Success = true,
                        Message = "Login successful",
                        Data = new UserInfo
                        {
                            UserId = result.Data.UserId,
                            Email = result.Data.Email,
                            FullName = result.Data.FullName,
                            Roles = result.Data.Roles,
                            EmailConfirmed = result.Data.EmailConfirmed
                        }
                    });
                }
            }

            var errorResult = JsonSerializer.Deserialize<ApiResponse<AuthResponse>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return BadRequest(errorResult);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = $"An error occurred: {ex.Message}"
            });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var client = _httpClientFactory.CreateClient();
        var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7176/api";

        try
        {
            var response = await client.PostAsJsonAsync($"{apiBaseUrl}/Auth/register", request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<AuthResponse>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result?.Success == true && result.Data != null)
                {
                    // Store tokens in HTTP-only cookies
                    SetAuthCookies(result.Data.AccessToken, result.Data.RefreshToken, false);

                    return Ok(new ApiResponse<UserInfo>
                    {
                        Success = true,
                        Message = result.Message ?? "Registration successful",
                        Data = new UserInfo
                        {
                            UserId = result.Data.UserId,
                            Email = result.Data.Email,
                            FullName = result.Data.FullName,
                            Roles = result.Data.Roles,
                            EmailConfirmed = result.Data.EmailConfirmed
                        }
                    });
                }
            }

            var errorResult = JsonSerializer.Deserialize<ApiResponse<AuthResponse>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return BadRequest(errorResult);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = $"An error occurred: {ex.Message}"
            });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Cookies[AuthCookieName];
        var refreshToken = Request.Cookies[RefreshCookieName];

        if (!string.IsNullOrEmpty(token))
        {
            var client = _httpClientFactory.CreateClient();
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7176/api";

            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                await client.PostAsJsonAsync($"{apiBaseUrl}/Auth/logout", new { RefreshToken = refreshToken });
            }
            catch { /* Ignore errors on logout */ }
        }

        // Clear cookies
        ClearAuthCookies();

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Logged out successfully"
        });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var token = Request.Cookies[AuthCookieName];

        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Not authenticated"
            });
        }

        var client = _httpClientFactory.CreateClient();
        var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7176/api";

        try
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"{apiBaseUrl}/Auth/me");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<UserInfo>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return Ok(result);
            }

            // Try to refresh token if unauthorized
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await RefreshTokenInternal();
                if (refreshed)
                {
                    // Retry with new token
                    var newToken = Request.Cookies[AuthCookieName];
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                    response = await client.GetAsync($"{apiBaseUrl}/Auth/me");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<ApiResponse<UserInfo>>(content,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        return Ok(result);
                    }
                }
            }

            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Authentication failed"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = $"An error occurred: {ex.Message}"
            });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshed = await RefreshTokenInternal();

        if (refreshed)
        {
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Token refreshed"
            });
        }

        ClearAuthCookies();
        return Unauthorized(new ApiResponse<object>
        {
            Success = false,
            Message = "Failed to refresh token"
        });
    }

    [HttpPost("proxy/{*path}")]
    public async Task<IActionResult> ProxyApiCall(string path)
    {
        var token = Request.Cookies[AuthCookieName];

        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Not authenticated"
            });
        }

        var client = _httpClientFactory.CreateClient();
        var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7176/api";

        try
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Forward the request to the backend API
            var requestMessage = new HttpRequestMessage
            {
                Method = new HttpMethod(Request.Method),
                RequestUri = new Uri($"{apiBaseUrl}/{path}{Request.QueryString}")
            };

            // Copy body if present
            if (Request.ContentLength > 0)
            {
                var body = await new StreamReader(Request.Body).ReadToEndAsync();
                requestMessage.Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
            }

            var response = await client.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = content,
                ContentType = "application/json",
                StatusCode = (int)response.StatusCode
            };
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = $"Proxy error: {ex.Message}"
            });
        }
    }

    private async Task<bool> RefreshTokenInternal()
    {
        var refreshToken = Request.Cookies[RefreshCookieName];

        if (string.IsNullOrEmpty(refreshToken))
            return false;

        var client = _httpClientFactory.CreateClient();
        var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7176/api";

        try
        {
            var response = await client.PostAsJsonAsync($"{apiBaseUrl}/Auth/refresh-token",
                new { RefreshToken = refreshToken });

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<AuthResponse>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result?.Success == true && result.Data != null)
                {
                    SetAuthCookies(result.Data.AccessToken, result.Data.RefreshToken, true);
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

    private void SetAuthCookies(string token, string refreshToken, bool persistent)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Requires HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = persistent ? DateTimeOffset.UtcNow.AddDays(30) : null
        };

        Response.Cookies.Append(AuthCookieName, token, cookieOptions);
        Response.Cookies.Append(RefreshCookieName, refreshToken, cookieOptions);
    }

    private void ClearAuthCookies()
    {
        Response.Cookies.Delete(AuthCookieName);
        Response.Cookies.Delete(RefreshCookieName);
    }
}

// DTOs (copy these from your existing AuthenticationService or create in a shared location)
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