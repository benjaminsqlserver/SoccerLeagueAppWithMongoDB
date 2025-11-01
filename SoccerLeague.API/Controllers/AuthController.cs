//============================================================================
// API/Controllers/AuthController.cs - COMPLETE IMPLEMENTATION
//============================================================================
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoccerLeague.API.Models;
using SoccerLeague.Application.DTOs.Auth;
using SoccerLeague.Application.DTOs.User;
using SoccerLeague.Application.Features.Auth.Commands.ForgotPassword;
using SoccerLeague.Application.Features.Auth.Commands.Login;
using SoccerLeague.Application.Features.Auth.Commands.Logout;
using SoccerLeague.Application.Features.Auth.Commands.RefreshToken;
using SoccerLeague.Application.Features.Auth.Commands.Register;
using SoccerLeague.Application.Features.Auth.Commands.ResetPassword;
using SoccerLeague.Application.Features.Auth.Commands.VerifyEmail;
using SoccerLeague.Application.Features.Auth.Commands.GoogleLogin;
using SoccerLeague.Application.Features.Auth.Commands.ResendVerificationEmail;
using SoccerLeague.Application.Features.Auth.Commands.RevokeToken;
using SoccerLeague.Application.Features.Users.Commands.ChangePassword;

namespace SoccerLeague.API.Controllers
{
    /// <summary>
    /// API Controller for authentication operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="registerDto">Registration details</param>
        /// <returns>Authentication response with tokens</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var command = new RegisterCommand
                {
                    RegisterDto = registerDto,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = Request.Headers["User-Agent"].ToString()
                };

                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<AuthResponseDto>.Error(
                        result.ErrorMessage ?? "Registration failed",
                        result.Errors));
                }

                _logger.LogInformation("User registered successfully: {Email}", registerDto.Email);

                return StatusCode(StatusCodes.Status201Created,
                    ApiResponse<AuthResponseDto>.SuccessResponse(
                        result.Data!,
                        "Registration successful. Please check your email to verify your account."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return StatusCode(500, ApiResponse<AuthResponseDto>.Error(
                    "An error occurred during registration"));
            }
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        /// <param name="loginDto">Login credentials</param>
        /// <returns>Authentication response with tokens</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var command = new LoginCommand
                {
                    LoginDto = loginDto,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = Request.Headers["User-Agent"].ToString()
                };

                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed login attempt for {Email}", loginDto.Email);
                    return Unauthorized(ApiResponse<AuthResponseDto>.Error(
                        result.ErrorMessage ?? "Login failed"));
                }

                _logger.LogInformation("User logged in successfully: {Email}", loginDto.Email);

                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
                    result.Data!,
                    "Login successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, ApiResponse<AuthResponseDto>.Error(
                    "An error occurred during login"));
            }
        }

        /// <summary>
        /// Login with Google OAuth
        /// </summary>
        /// <param name="googleLoginDto">Google ID token</param>
        /// <returns>Authentication response with tokens</returns>
        [HttpPost("google-login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> GoogleLogin([FromBody] GoogleLoginDto googleLoginDto)
        {
            try
            {
                var command = new GoogleLoginCommand
                {
                    GoogleLoginDto = googleLoginDto,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = Request.Headers["User-Agent"].ToString()
                };

                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed Google login attempt");
                    return BadRequest(ApiResponse<AuthResponseDto>.Error(
                        result.ErrorMessage ?? "Google login failed",
                        result.Errors));
                }

                _logger.LogInformation("User logged in successfully via Google: {Email}", result.Data!.Email);

                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
                    result.Data!,
                    "Google login successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Google login");
                return StatusCode(500, ApiResponse<AuthResponseDto>.Error(
                    "An error occurred during Google login"));
            }
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        /// <param name="refreshTokenDto">Refresh token</param>
        /// <returns>New authentication tokens</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var command = new RefreshTokenCommand
                {
                    RefreshToken = refreshTokenDto.RefreshToken,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = Request.Headers["User-Agent"].ToString()
                };

                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return Unauthorized(ApiResponse<AuthResponseDto>.Error(
                        result.ErrorMessage ?? "Token refresh failed"));
                }

                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
                    result.Data!,
                    "Token refreshed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return StatusCode(500, ApiResponse<AuthResponseDto>.Error(
                    "An error occurred during token refresh"));
            }
        }

        /// <summary>
        /// Logout current user
        /// </summary>
        /// <param name="refreshTokenDto">Optional refresh token to revoke specific session</param>
        /// <returns>Success status</returns>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<bool>>> Logout([FromBody] RefreshTokenDto? refreshTokenDto)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse<bool>.Error("User not authenticated"));
                }

                var command = new LogoutCommand
                {
                    UserId = userId,
                    RefreshToken = refreshTokenDto?.RefreshToken
                };

                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.Error(
                        result.ErrorMessage ?? "Logout failed"));
                }

                _logger.LogInformation("User logged out successfully: {UserId}", userId);

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Logout successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, ApiResponse<bool>.Error(
                    "An error occurred during logout"));
            }
        }

        /// <summary>
        /// Request password reset email
        /// </summary>
        /// <param name="forgotPasswordDto">Email address</param>
        /// <returns>Success status</returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var command = new ForgotPasswordCommand
                {
                    ForgotPasswordDto = forgotPasswordDto
                };

                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.Error(
                        result.ErrorMessage ?? "Request failed",
                        result.Errors));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(
                    true,
                    "If an account exists with this email, you will receive a password reset link."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password request");
                return StatusCode(500, ApiResponse<bool>.Error(
                    "An error occurred while processing your request"));
            }
        }

        /// <summary>
        /// Reset password using reset token
        /// </summary>
        /// <param name="resetPasswordDto">Reset password details</param>
        /// <returns>Success status</returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var command = new ResetPasswordCommand
                {
                    ResetPasswordDto = resetPasswordDto
                };

                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.Error(
                        result.ErrorMessage ?? "Password reset failed",
                        result.Errors));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(
                    true,
                    "Password reset successful. You can now login with your new password."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset");
                return StatusCode(500, ApiResponse<bool>.Error(
                    "An error occurred during password reset"));
            }
        }

        /// <summary>
        /// Verify email address
        /// </summary>
        /// <param name="verifyEmailDto">Verification token</param>
        /// <returns>Success status</returns>
        [HttpPost("verify-email")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> VerifyEmail([FromBody] VerifyEmailDto verifyEmailDto)
        {
            try
            {
                var command = new VerifyEmailCommand
                {
                    VerifyEmailDto = verifyEmailDto
                };

                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.Error(
                        result.ErrorMessage ?? "Email verification failed"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(
                    true,
                    "Email verified successfully!"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email verification");
                return StatusCode(500, ApiResponse<bool>.Error(
                    "An error occurred during email verification"));
            }
        }

        /// <summary>
        /// Resend email verification link
        /// </summary>
        /// <param name="resendDto">Email address</param>
        /// <returns>Success status</returns>
        [HttpPost("resend-verification-email")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> ResendVerificationEmail([FromBody] ResendVerificationEmailDto resendDto)
        {
            try
            {
                var command = new ResendVerificationEmailCommand
                {
                    ResendDto = resendDto
                };

                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.Error(
                        result.ErrorMessage ?? "Failed to resend verification email",
                        result.Errors));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(
                    true,
                    "If an unverified account exists with this email, a new verification link has been sent."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending verification email");
                return StatusCode(500, ApiResponse<bool>.Error(
                    "An error occurred while processing your request"));
            }
        }

        /// <summary>
        /// Get current authenticated user information
        /// </summary>
        /// <returns>User information</returns>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public ActionResult<ApiResponse<object>> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var name = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();
                var emailConfirmed = User.FindFirst("email_confirmed")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse<object>.Error("User not authenticated"));
                }

                var userData = new
                {
                    userId,
                    email,
                    name,
                    roles,
                    emailConfirmed = bool.Parse(emailConfirmed ?? "false")
                };

                return Ok(ApiResponse<object>.SuccessResponse(userData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return StatusCode(500, ApiResponse<object>.Error(
                    "An error occurred while retrieving user information"));
            }
        }

        /// <summary>
        /// Change password for authenticated user
        /// </summary>
        /// <param name="changePasswordDto">Current and new password</param>
        /// <returns>Success status</returns>
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse<bool>.Error("User not authenticated"));
                }

                // Ensure the userId from token matches the request
                if (userId != changePasswordDto.UserId)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        ApiResponse<bool>.Error("You can only change your own password"));
                }

                var command = new ChangePasswordCommand
                {
                    PasswordData = changePasswordDto
                };

                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.Error(
                        result.ErrorMessage ?? "Failed to change password",
                        result.Errors));
                }

                _logger.LogInformation("Password changed successfully for user: {UserId}", userId);

                return Ok(ApiResponse<bool>.SuccessResponse(
                    true,
                    "Password changed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, ApiResponse<bool>.Error(
                    "An error occurred while changing password"));
            }
        }

        /// <summary>
        /// Revoke a specific refresh token (terminate session)
        /// </summary>
        /// <param name="refreshTokenDto">Refresh token to revoke</param>
        /// <returns>Success status</returns>
        [HttpPost("revoke-token")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<bool>>> RevokeToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse<bool>.Error("User not authenticated"));
                }

                var command = new RevokeTokenCommand
                {
                    RefreshToken = refreshTokenDto.RefreshToken,
                    UserId = userId
                };

                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.Error(
                        result.ErrorMessage ?? "Failed to revoke token",
                        result.Errors));
                }

                _logger.LogInformation("Token revoked successfully for user: {UserId}", userId);

                return Ok(ApiResponse<bool>.SuccessResponse(
                    true,
                    "Token revoked successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking token");
                return StatusCode(500, ApiResponse<bool>.Error(
                    "An error occurred while revoking token"));
            }
        }

        /// <summary>
        /// Validate current access token (health check for authentication)
        /// </summary>
        /// <returns>Token validity status</returns>
        [HttpGet("validate-token")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public ActionResult<ApiResponse<object>> ValidateToken()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse<object>.Error("Invalid token"));
                }

                var expClaim = User.FindFirst("exp")?.Value;
                DateTime? expiresAt = null;

                if (!string.IsNullOrEmpty(expClaim) && long.TryParse(expClaim, out long exp))
                {
                    expiresAt = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                }

                var tokenInfo = new
                {
                    valid = true,
                    userId,
                    expiresAt,
                    isExpired = expiresAt.HasValue && expiresAt.Value < DateTime.UtcNow
                };

                return Ok(ApiResponse<object>.SuccessResponse(tokenInfo, "Token is valid"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return StatusCode(500, ApiResponse<object>.Error(
                    "An error occurred while validating token"));
            }
        }
    }
}