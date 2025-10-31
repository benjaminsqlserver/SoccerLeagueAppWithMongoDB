namespace SoccerLeague.API.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using SoccerLeague.API.Models;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.User;
    using SoccerLeague.Application.Features.Users.Commands.CreateUser;
    using SoccerLeague.Application.Features.Users.Commands.UpdateUser;
    using SoccerLeague.Application.Features.Users.Commands.DeleteUser;
    using SoccerLeague.Application.Features.Users.Commands.ChangePassword;
    using SoccerLeague.Application.Features.Users.Queries.GetAllUsers;
    using SoccerLeague.Application.Features.Users.Queries.GetUserById;
    using SoccerLeague.Application.Features.Users.Queries.GetUserByEmail;
    using SoccerLeague.Application.Features.Users.Queries.GetActiveUsers;

    /// <summary>
    /// API Controller for managing users
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMediator mediator, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of all users with optional filtering
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of users</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<UserDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<UserDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetAllUsers([FromQuery] UserQueryParameters parameters)
        {
            try
            {
                var query = new GetAllUsersQuery { Parameters = parameters };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<PagedResult<UserDto>>.Error(result.ErrorMessage ?? "Failed to retrieve users"));
                }

                return Ok(ApiResponse<PagedResult<UserDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, ApiResponse<PagedResult<UserDto>>.Error("An error occurred while retrieving users"));
            }
        }

        /// <summary>
        /// Gets a specific user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(string id)
        {
            try
            {
                var query = new GetUserByIdQuery { Id = id };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<UserDto>.Error(result.ErrorMessage ?? "User not found"));
                }

                return Ok(ApiResponse<UserDto>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {Id}", id);
                return StatusCode(500, ApiResponse<UserDto>.Error("An error occurred while retrieving the user"));
            }
        }

        /// <summary>
        /// Gets a user by email address
        /// </summary>
        /// <param name="email">User email address</param>
        /// <returns>User details</returns>
        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserByEmail(string email)
        {
            try
            {
                var query = new GetUserByEmailQuery { Email = email };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<UserDto>.Error(result.ErrorMessage ?? "User not found"));
                }

                return Ok(ApiResponse<UserDto>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with email {Email}", email);
                return StatusCode(500, ApiResponse<UserDto>.Error("An error occurred while retrieving the user"));
            }
        }

        /// <summary>
        /// Gets all active users
        /// </summary>
        /// <returns>List of active users</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<UserDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetActiveUsers()
        {
            try
            {
                var query = new GetActiveUsersQuery();
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<UserDto>>.Error(result.ErrorMessage ?? "Failed to retrieve active users"));
                }

                return Ok(ApiResponse<List<UserDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active users");
                return StatusCode(500, ApiResponse<List<UserDto>>.Error("An error occurred while retrieving active users"));
            }
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="createDto">User creation details</param>
        /// <returns>Created user details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserDto createDto)
        {
            try
            {
                var command = new CreateUserCommand { User = createDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<UserDto>.Error(result.ErrorMessage ?? "Failed to create user", result.Errors));
                }

                return CreatedAtAction(
                    nameof(GetUserById),
                    new { id = result.Data!.Id },
                    ApiResponse<UserDto>.SuccessResponse(result.Data, "User created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, ApiResponse<UserDto>.Error("An error occurred while creating the user"));
            }
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="updateDto">User update details</param>
        /// <returns>Updated user details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(string id, [FromBody] UpdateUserDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return BadRequest(ApiResponse<UserDto>.Error("User ID mismatch"));
                }

                var command = new UpdateUserCommand { User = updateDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage?.Contains("not found") == true)
                    {
                        return NotFound(ApiResponse<UserDto>.Error(result.ErrorMessage));
                    }
                    return BadRequest(ApiResponse<UserDto>.Error(result.ErrorMessage ?? "Failed to update user", result.Errors));
                }

                return Ok(ApiResponse<UserDto>.SuccessResponse(result.Data!, "User updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {Id}", id);
                return StatusCode(500, ApiResponse<UserDto>.Error("An error occurred while updating the user"));
            }
        }

        /// <summary>
        /// Deletes a user (soft delete)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(string id)
        {
            try
            {
                var command = new DeleteUserCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<bool>.Error(result.ErrorMessage ?? "User not found"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "User deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {Id}", id);
                return StatusCode(500, ApiResponse<bool>.Error("An error occurred while deleting the user"));
            }
        }

        /// <summary>
        /// Changes a user's password
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="changePasswordDto">Password change details</param>
        /// <returns>Success status</returns>
        [HttpPost("{userId}/change-password")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword(string userId, [FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (userId != changePasswordDto.UserId)
                {
                    return BadRequest(ApiResponse<bool>.Error("User ID mismatch"));
                }

                var command = new ChangePasswordCommand { PasswordData = changePasswordDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage?.Contains("not found") == true)
                    {
                        return NotFound(ApiResponse<bool>.Error(result.ErrorMessage));
                    }
                    return BadRequest(ApiResponse<bool>.Error(result.ErrorMessage ?? "Failed to change password", result.Errors));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Password changed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return StatusCode(500, ApiResponse<bool>.Error("An error occurred while changing the password"));
            }
        }
    }
}