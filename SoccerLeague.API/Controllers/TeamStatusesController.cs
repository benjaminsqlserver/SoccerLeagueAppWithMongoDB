namespace SoccerLeague.API.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using SoccerLeague.API.Models;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.TeamStatus;
    using SoccerLeague.Application.Features.TeamStatuses.Commands.CreateTeamStatus;
    using SoccerLeague.Application.Features.TeamStatuses.Commands.UpdateTeamStatus;
    using SoccerLeague.Application.Features.TeamStatuses.Commands.DeleteTeamStatus;
    using SoccerLeague.Application.Features.TeamStatuses.Queries.GetAllTeamStatuses;
    using SoccerLeague.Application.Features.TeamStatuses.Queries.GetTeamStatusById;
    using SoccerLeague.Application.Features.TeamStatuses.Queries.GetActiveTeamStatuses;

    /// <summary>
    /// API Controller for managing team statuses (Active, Inactive, Suspended)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TeamStatusesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TeamStatusesController> _logger;

        public TeamStatusesController(IMediator mediator, ILogger<TeamStatusesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of all team statuses
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of team statuses</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<TeamStatusDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<TeamStatusDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PagedResult<TeamStatusDto>>>> GetAllTeamStatuses([FromQuery] QueryParameters parameters)
        {
            try
            {
                var query = new GetAllTeamStatusesQuery { Parameters = parameters };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<PagedResult<TeamStatusDto>>.Error(result.ErrorMessage ?? "Failed to retrieve team statuses"));
                }

                return Ok(ApiResponse<PagedResult<TeamStatusDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving team statuses");
                return StatusCode(500, ApiResponse<PagedResult<TeamStatusDto>>.Error("An error occurred while retrieving team statuses"));
            }
        }

        /// <summary>
        /// Gets a specific team status by ID
        /// </summary>
        /// <param name="id">Team status ID</param>
        /// <returns>Team status details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TeamStatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<TeamStatusDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<TeamStatusDto>>> GetTeamStatusById(string id)
        {
            try
            {
                var query = new GetTeamStatusByIdQuery { Id = id };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<TeamStatusDto>.Error(result.ErrorMessage ?? "Team status not found"));
                }

                return Ok(ApiResponse<TeamStatusDto>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving team status {Id}", id);
                return StatusCode(500, ApiResponse<TeamStatusDto>.Error("An error occurred while retrieving the team status"));
            }
        }

        /// <summary>
        /// Gets all active team statuses
        /// </summary>
        /// <returns>List of active team statuses ordered by display order</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<TeamStatusDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<TeamStatusDto>>>> GetActiveTeamStatuses()
        {
            try
            {
                var query = new GetActiveTeamStatusesQuery();
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<TeamStatusDto>>.Error(result.ErrorMessage ?? "Failed to retrieve active team statuses"));
                }

                return Ok(ApiResponse<List<TeamStatusDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active team statuses");
                return StatusCode(500, ApiResponse<List<TeamStatusDto>>.Error("An error occurred while retrieving active team statuses"));
            }
        }

        /// <summary>
        /// Creates a new team status
        /// </summary>
        /// <param name="createDto">Team status creation details</param>
        /// <returns>Created team status details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<TeamStatusDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<TeamStatusDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<TeamStatusDto>>> CreateTeamStatus([FromBody] CreateTeamStatusDto createDto)
        {
            try
            {
                var command = new CreateTeamStatusCommand { TeamStatus = createDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<TeamStatusDto>.Error(result.ErrorMessage ?? "Failed to create team status", result.Errors));
                }

                return CreatedAtAction(
                    nameof(GetTeamStatusById),
                    new { id = result.Data!.Id },
                    ApiResponse<TeamStatusDto>.SuccessResponse(result.Data, "Team status created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating team status");
                return StatusCode(500, ApiResponse<TeamStatusDto>.Error("An error occurred while creating the team status"));
            }
        }

        /// <summary>
        /// Updates an existing team status
        /// </summary>
        /// <param name="id">Team status ID</param>
        /// <param name="updateDto">Team status update details</param>
        /// <returns>Updated team status details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TeamStatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<TeamStatusDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<TeamStatusDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<TeamStatusDto>>> UpdateTeamStatus(string id, [FromBody] UpdateTeamStatusDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return BadRequest(ApiResponse<TeamStatusDto>.Error("Team status ID mismatch"));
                }

                var command = new UpdateTeamStatusCommand { TeamStatus = updateDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage?.Contains("not found") == true)
                    {
                        return NotFound(ApiResponse<TeamStatusDto>.Error(result.ErrorMessage));
                    }
                    return BadRequest(ApiResponse<TeamStatusDto>.Error(result.ErrorMessage ?? "Failed to update team status", result.Errors));
                }

                return Ok(ApiResponse<TeamStatusDto>.SuccessResponse(result.Data!, "Team status updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating team status {Id}", id);
                return StatusCode(500, ApiResponse<TeamStatusDto>.Error("An error occurred while updating the team status"));
            }
        }

        /// <summary>
        /// Deletes a team status (soft delete)
        /// </summary>
        /// <param name="id">Team status ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTeamStatus(string id)
        {
            try
            {
                var command = new DeleteTeamStatusCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<bool>.Error(result.ErrorMessage ?? "Team status not found"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Team status deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting team status {Id}", id);
                return StatusCode(500, ApiResponse<bool>.Error("An error occurred while deleting the team status"));
            }
        }
    }
}