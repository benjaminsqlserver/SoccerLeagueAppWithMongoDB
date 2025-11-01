using MediatR;
using Microsoft.AspNetCore.Mvc;
using SoccerLeague.API.Models;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.AuditLog;
using SoccerLeague.Application.Features.AuditLogs.Commands.CreateAuditLog;
using SoccerLeague.Application.Features.AuditLogs.Commands.DeleteOldAuditLogs;
using SoccerLeague.Application.Features.AuditLogs.Queries.GetAllAuditLogs;
using SoccerLeague.Application.Features.AuditLogs.Queries.GetAuditLogById;
using SoccerLeague.Application.Features.AuditLogs.Queries.GetAuditLogsByUser;
using SoccerLeague.Application.Features.AuditLogs.Queries.GetAuditLogsByEntity;
using SoccerLeague.Application.Features.AuditLogs.Queries.GetRecentActivity;
using SoccerLeague.Application.Features.AuditLogs.Queries.GetSecurityEvents;
using SoccerLeague.Application.Features.AuditLogs.Queries.GetFailedActions;

namespace SoccerLeague.API.Controllers
{
    /// <summary>
    /// API Controller for managing audit logs
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuditLogsController> _logger;

        public AuditLogsController(IMediator mediator, ILogger<AuditLogsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of all audit logs with optional filtering
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of audit logs</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<AuditLogDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<AuditLogDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PagedResult<AuditLogDto>>>> GetAllAuditLogs([FromQuery] AuditLogQueryParameters parameters)
        {
            try
            {
                var query = new GetAllAuditLogsQuery { Parameters = parameters };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<PagedResult<AuditLogDto>>.Error(result.ErrorMessage ?? "Failed to retrieve audit logs"));
                }

                return Ok(ApiResponse<PagedResult<AuditLogDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit logs");
                return StatusCode(500, ApiResponse<PagedResult<AuditLogDto>>.Error("An error occurred while retrieving audit logs"));
            }
        }

        /// <summary>
        /// Gets a specific audit log by ID
        /// </summary>
        /// <param name="id">Audit log ID</param>
        /// <returns>Audit log details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AuditLogDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuditLogDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<AuditLogDto>>> GetAuditLogById(string id)
        {
            try
            {
                var query = new GetAuditLogByIdQuery { Id = id };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<AuditLogDto>.Error(result.ErrorMessage ?? "Audit log not found"));
                }

                return Ok(ApiResponse<AuditLogDto>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit log {Id}", id);
                return StatusCode(500, ApiResponse<AuditLogDto>.Error("An error occurred while retrieving the audit log"));
            }
        }

        /// <summary>
        /// Gets audit logs for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="limit">Maximum number of records to return (default: 100)</param>
        /// <returns>List of audit logs for the user</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<List<AuditLogDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<AuditLogDto>>>> GetAuditLogsByUser(string userId, [FromQuery] int limit = 100)
        {
            try
            {
                var query = new GetAuditLogsByUserQuery { UserId = userId, Limit = limit };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<AuditLogDto>>.Error(result.ErrorMessage ?? "Failed to retrieve user audit logs"));
                }

                return Ok(ApiResponse<List<AuditLogDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit logs for user {UserId}", userId);
                return StatusCode(500, ApiResponse<List<AuditLogDto>>.Error("An error occurred while retrieving user audit logs"));
            }
        }

        /// <summary>
        /// Gets audit logs for a specific entity
        /// </summary>
        /// <param name="entityType">Entity type (e.g., "User", "Match", "Team")</param>
        /// <param name="entityId">Entity ID</param>
        /// <returns>List of audit logs for the entity</returns>
        [HttpGet("entity/{entityType}/{entityId}")]
        [ProducesResponseType(typeof(ApiResponse<List<AuditLogDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<AuditLogDto>>>> GetAuditLogsByEntity(string entityType, string entityId)
        {
            try
            {
                var query = new GetAuditLogsByEntityQuery { EntityType = entityType, EntityId = entityId };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<AuditLogDto>>.Error(result.ErrorMessage ?? "Failed to retrieve entity audit logs"));
                }

                return Ok(ApiResponse<List<AuditLogDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit logs for entity {EntityType}/{EntityId}", entityType, entityId);
                return StatusCode(500, ApiResponse<List<AuditLogDto>>.Error("An error occurred while retrieving entity audit logs"));
            }
        }

        /// <summary>
        /// Gets recent activity across the system
        /// </summary>
        /// <param name="limit">Maximum number of records to return (default: 50)</param>
        /// <returns>List of recent audit logs</returns>
        [HttpGet("recent")]
        [ProducesResponseType(typeof(ApiResponse<List<AuditLogDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<AuditLogDto>>>> GetRecentActivity([FromQuery] int limit = 50)
        {
            try
            {
                var query = new GetRecentActivityQuery { Limit = limit };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<AuditLogDto>>.Error(result.ErrorMessage ?? "Failed to retrieve recent activity"));
                }

                return Ok(ApiResponse<List<AuditLogDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent activity");
                return StatusCode(500, ApiResponse<List<AuditLogDto>>.Error("An error occurred while retrieving recent activity"));
            }
        }

        /// <summary>
        /// Gets security-related events (logins, password changes, lockouts, etc.)
        /// </summary>
        /// <param name="limit">Maximum number of records to return (default: 100)</param>
        /// <returns>List of security event audit logs</returns>
        [HttpGet("security-events")]
        [ProducesResponseType(typeof(ApiResponse<List<AuditLogDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<AuditLogDto>>>> GetSecurityEvents([FromQuery] int limit = 100)
        {
            try
            {
                var query = new GetSecurityEventsQuery { Limit = limit };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<AuditLogDto>>.Error(result.ErrorMessage ?? "Failed to retrieve security events"));
                }

                return Ok(ApiResponse<List<AuditLogDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving security events");
                return StatusCode(500, ApiResponse<List<AuditLogDto>>.Error("An error occurred while retrieving security events"));
            }
        }

        /// <summary>
        /// Gets failed actions for troubleshooting
        /// </summary>
        /// <param name="limit">Maximum number of records to return (default: 100)</param>
        /// <returns>List of failed action audit logs</returns>
        [HttpGet("failed-actions")]
        [ProducesResponseType(typeof(ApiResponse<List<AuditLogDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<AuditLogDto>>>> GetFailedActions([FromQuery] int limit = 100)
        {
            try
            {
                var query = new GetFailedActionsQuery { Limit = limit };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<AuditLogDto>>.Error(result.ErrorMessage ?? "Failed to retrieve failed actions"));
                }

                return Ok(ApiResponse<List<AuditLogDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving failed actions");
                return StatusCode(500, ApiResponse<List<AuditLogDto>>.Error("An error occurred while retrieving failed actions"));
            }
        }

        /// <summary>
        /// Creates a new audit log entry
        /// </summary>
        /// <param name="createDto">Audit log creation details</param>
        /// <returns>Created audit log details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AuditLogDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<AuditLogDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<AuditLogDto>>> CreateAuditLog([FromBody] CreateAuditLogDto createDto)
        {
            try
            {
                var command = new CreateAuditLogCommand { AuditLog = createDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<AuditLogDto>.Error(result.ErrorMessage ?? "Failed to create audit log", result.Errors));
                }

                return CreatedAtAction(
                    nameof(GetAuditLogById),
                    new { id = result.Data!.Id },
                    ApiResponse<AuditLogDto>.SuccessResponse(result.Data, "Audit log created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating audit log");
                return StatusCode(500, ApiResponse<AuditLogDto>.Error("An error occurred while creating the audit log"));
            }
        }

        /// <summary>
        /// Deletes audit logs older than a specified date (for data retention compliance)
        /// </summary>
        /// <param name="olderThan">Delete logs older than this date</param>
        /// <returns>Success status</returns>
        [HttpDelete("cleanup")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteOldAuditLogs([FromQuery] DateTime olderThan)
        {
            try
            {
                var command = new DeleteOldAuditLogsCommand { OlderThan = olderThan };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.Error(result.ErrorMessage ?? "Failed to delete old audit logs"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Old audit logs deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting old audit logs");
                return StatusCode(500, ApiResponse<bool>.Error("An error occurred while deleting old audit logs"));
            }
        }
    }
}
