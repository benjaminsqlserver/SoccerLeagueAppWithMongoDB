using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SoccerLeague.Application.Contracts.Services;

namespace SoccerLeague.Infrastructure.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleAuthService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _googleClientId;

        public GoogleAuthService(
            IConfiguration configuration,
            ILogger<GoogleAuthService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _googleClientId = configuration["Google:ClientId"] ?? throw new InvalidOperationException("Google ClientId not configured");
        }

        public async Task<GoogleUserInfo?> ValidateGoogleTokenAsync(string idToken)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to validate Google token. Status: {Status}", response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var tokenInfo = JsonSerializer.Deserialize<JsonElement>(content);

                // Verify the token is for our client
                var aud = tokenInfo.GetProperty("aud").GetString();
                if (aud != _googleClientId)
                {
                    _logger.LogWarning("Google token audience mismatch");
                    return null;
                }

                return new GoogleUserInfo
                {
                    Sub = tokenInfo.GetProperty("sub").GetString() ?? "",
                    Email = tokenInfo.GetProperty("email").GetString() ?? "",
                    EmailVerified = tokenInfo.GetProperty("email_verified").GetString() == "true",
                    Name = tokenInfo.GetProperty("name").GetString() ?? "",
                    GivenName = tokenInfo.GetProperty("given_name").GetString() ?? "",
                    FamilyName = tokenInfo.GetProperty("family_name").GetString() ?? "",
                    Picture = tokenInfo.GetProperty("picture").GetString() ?? ""
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Google token");
                return null;
            }
        }
    }
}
