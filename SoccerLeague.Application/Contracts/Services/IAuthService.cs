using SoccerLeague.Application.DTOs.Auth;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Contracts.Services
{
    public interface IAuthService
    {
        Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto registerDto, string? ipAddress = null, string? userAgent = null);
        Task<Result<AuthResponseDto>> LoginAsync(LoginDto loginDto, string? ipAddress = null, string? userAgent = null);
        Task<Result<AuthResponseDto>> GoogleLoginAsync(GoogleLoginDto googleLoginDto, string? ipAddress = null, string? userAgent = null);
        Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken, string? ipAddress = null, string? userAgent = null);
        Task<Result<bool>> LogoutAsync(string userId, string? refreshToken = null);
        Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<Result<bool>> VerifyEmailAsync(VerifyEmailDto verifyEmailDto);
        Task<Result<bool>> ResendVerificationEmailAsync(ResendVerificationEmailDto resendDto);
        Task<Result<bool>> RevokeTokenAsync(string refreshToken);
    }
}
