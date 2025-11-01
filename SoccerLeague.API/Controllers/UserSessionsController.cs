using MediatR;
using Microsoft.AspNetCore.Mvc;
using SoccerLeague.API.Models;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.UserSession;
using SoccerLeague.Application.Features.UserSessions.Commands.CreateUserSession;
using SoccerLeague.Application.Features.UserSessions.Commands.TerminateSession;
using SoccerLeague.Application.Features.UserSessions.Commands.TerminateAllUserSessions;
using SoccerLeague.Application.Features.UserSessions.Queries.GetAllSessions;
using SoccerLeague.Application.Features.UserSessions.Queries.GetSessionById;
using SoccerLeague.Application.Features.UserSessions.Queries.GetActiveUserSessions;
using SoccerLeague.Application.Features.UserSessions.Queries.GetUserSessions;

namespace SoccerLeague.API.Controllers
{
    /// <summary>
    /// API Controller for managing user sessions
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserSessionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserSessionsController> _logger;

        public UserSessionsController(IMediator mediator, ILogger<UserSessionsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of all sessions with optional filtering
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of sessions</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<UserSessionDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<UserSessionDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PagedResult<UserSessionDto>>>> GetAllSessions([FromQuery] UserSessionQueryParameters parameters)
        {
            try
            {
                var query = new GetAllSessionsQuery { Parameters = parameters };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<PagedResult<UserSessionDto>>.Error(result.ErrorMessage ?? "Failed to retrieve sessions"));
                }

                return Ok(ApiResponse<PagedResult<UserSessionDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sessions");
                return StatusCode(500, ApiResponse<PagedResult<UserSessionDto>>.Error("An error occurred while retrieving sessions"));
            }
        }

        /// <summary>
        /// Gets a specific session by ID
        /// </summary>
        /// <param name="id">Session ID</param>
        /// <returns>Session details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserSessionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<UserSessionDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UserSessionDto>>> GetSessionById(string id)
        {
            try
            {
                var query = new GetSessionByIdQuery { Id = id };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<UserSessionDto>.Error(result.ErrorMessage ?? "Session not found"));
                }

                return Ok(ApiResponse<UserSessionDto>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving session {Id}", id);
                return StatusCode(500, ApiResponse<UserSessionDto>.Error("An error occurred while retrieving the session"));
            }
        }

        /// <summary>
        /// Gets all active sessions for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of active sessions</returns>
        [HttpGet("user/{userId}/active")]
        [ProducesResponseType(typeof(ApiResponse<List<UserSessionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<UserSessionDto>>>> GetActiveUserSessions(string userId)
        {
            try
            {
                var query = new GetActiveUserSessionsQuery { UserId = userId };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<UserSessionDto>>.Error(result.ErrorMessage ?? "Failed to retrieve active sessions"));
                }

                return Ok(ApiResponse<List<UserSessionDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active sessions for user {UserId}", userId);
                return StatusCode(500, ApiResponse<List<UserSessionDto>>.Error("An error occurred while retrieving active sessions"));
            }
        }

        /// <summary>
        /// Gets all sessions (active and terminated) for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of all user sessions</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<List<UserSessionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<UserSessionDto>>>> GetUserSessions(string userId)
        {
            try
            {
                var query = new GetUserSessionsQuery { UserId = userId };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<UserSessionDto>>.Error(result.ErrorMessage ?? "Failed to retrieve user sessions"));
                }

                return Ok(ApiResponse<List<UserSessionDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sessions for user {UserId}", userId);
                return StatusCode(500, ApiResponse<List<UserSessionDto>>.Error("An error occurred while retrieving user sessions"));
            }
        }

        /// <summary>
        /// Creates a new session
        /// </summary>
        /// <param name="createDto">Session creation details</param>
        /// <returns>Created session details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserSessionDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<UserSessionDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<UserSessionDto>>> CreateSession([FromBody] CreateUserSessionDto createDto)
        {
            try
            {
                var command = new CreateUserSessionCommand { Session = createDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<UserSessionDto>.Error(result.ErrorMessage ?? "Failed to create session", result.Errors));
                }

                return CreatedAtAction(
                    nameof(GetSessionById),
                    new { id = result.Data!.Id },
                    ApiResponse<UserSessionDto>.SuccessResponse(result.Data, "Session created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating session");
                return StatusCode(500, ApiResponse<UserSessionDto>.Error("An error occurred while creating the session"));
            }
        }

        /// <summary>
        /// Terminates a specific session
        /// </summary>
        /// <param name="id">Session ID</param>
        /// <param name="terminateDto">Termination details</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/terminate")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> TerminateSession(string id, [FromBody] TerminateSessionDto terminateDto)
        {
            try
            {
                if (id != terminateDto.SessionId)
                {
                    return BadRequest(ApiResponse<bool>.Error("Session ID mismatch"));
                }

                var command = new TerminateSessionCommand { TerminateData = terminateDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage?.Contains("not found") == true)
                    {
                        return NotFound(ApiResponse<bool>.Error(result.ErrorMessage));
                    }
                    return BadRequest(ApiResponse<bool>.Error(result.ErrorMessage ?? "Failed to terminate session"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Session terminated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error terminating session {Id}", id);
                return StatusCode(500, ApiResponse<bool>.Error("An error occurred while terminating the session"));
            }
        }

        /// <summary>
        /// Terminates all active sessions for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="terminationReason">Reason for termination</param>
        /// <returns>Success status</returns>
        [HttpPost("user/{userId}/terminate-all")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> TerminateAllUserSessions(string userId, [FromBody] string terminationReason)
        {
            try
            {
                var command = new TerminateAllUserSessionsCommand
                {
                    UserId = userId,
                    TerminationReason = terminationReason ?? "Manual termination by user"
                };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.Error(result.ErrorMessage ?? "Failed to terminate sessions"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "All user sessions terminated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error terminating all sessions for user {UserId}", userId);
                return StatusCode(500, ApiResponse<bool>.Error("An error occurred while terminating user sessions"));
            }
        }

        /// <summary>
        /// Gets the count of active sessions for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Number of active sessions</returns>
        [HttpGet("user/{userId}/active-count")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<int>>> GetActiveSessionCount(string userId)
        {
            try
            {
                // This would require adding a new query handler
                // For now, we can get the sessions and count them
                var query = new GetActiveUserSessionsQuery { UserId = userId };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<int>.Error(result.ErrorMessage ?? "Failed to retrieve session count"));
                }

                var count = result.Data?.Count ?? 0;
                return Ok(ApiResponse<int>.SuccessResponse(count));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active session count for user {UserId}", userId);
                return StatusCode(500, ApiResponse<int>.Error("An error occurred while retrieving session count"));
            }
        }
    }
}
