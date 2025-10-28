namespace SoccerLeague.API.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using SoccerLeague.API.Models;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.MatchStatus;
    using SoccerLeague.Application.Features.MatchStatuses.Commands.CreateMatchStatus;
    using SoccerLeague.Application.Features.MatchStatuses.Commands.UpdateMatchStatus;
    using SoccerLeague.Application.Features.MatchStatuses.Commands.DeleteMatchStatus;
    using SoccerLeague.Application.Features.MatchStatuses.Queries.GetAllMatchStatuses;
    using SoccerLeague.Application.Features.MatchStatuses.Queries.GetMatchStatusById;
    using SoccerLeague.Application.Features.MatchStatuses.Queries.GetActiveMatchStatuses;

    /// <summary>
    /// API Controller for managing match statuses (Scheduled, In Progress, Completed, etc.)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MatchStatusesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MatchStatusesController> _logger;

        public MatchStatusesController(IMediator mediator, ILogger<MatchStatusesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of all match statuses
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of match statuses</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<MatchStatusDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<MatchStatusDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PagedResult<MatchStatusDto>>>> GetAllMatchStatuses([FromQuery] QueryParameters parameters)
        {
            try
            {
                var query = new GetAllMatchStatusesQuery { Parameters = parameters };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<PagedResult<MatchStatusDto>>.Error(result.ErrorMessage ?? "Failed to retrieve match statuses"));
                }

                return Ok(ApiResponse<PagedResult<MatchStatusDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving match statuses");
                return StatusCode(500, ApiResponse<PagedResult<MatchStatusDto>>.Error("An error occurred while retrieving match statuses"));
            }
        }

        /// <summary>
        /// Gets a specific match status by ID
        /// </summary>
        /// <param name="id">Match status ID</param>
        /// <returns>Match status details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<MatchStatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<MatchStatusDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<MatchStatusDto>>> GetMatchStatusById(string id)
        {
            try
            {
                var query = new GetMatchStatusByIdQuery { Id = id };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<MatchStatusDto>.Error(result.ErrorMessage ?? "Match status not found"));
                }

                return Ok(ApiResponse<MatchStatusDto>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving match status {Id}", id);
                return StatusCode(500, ApiResponse<MatchStatusDto>.Error("An error occurred while retrieving the match status"));
            }
        }

        /// <summary>
        /// Gets all active match statuses
        /// </summary>
        /// <returns>List of active match statuses ordered by display order</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<MatchStatusDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<MatchStatusDto>>>> GetActiveMatchStatuses()
        {
            try
            {
                var query = new GetActiveMatchStatusesQuery();
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<MatchStatusDto>>.Error(result.ErrorMessage ?? "Failed to retrieve active match statuses"));
                }

                return Ok(ApiResponse<List<MatchStatusDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active match statuses");
                return StatusCode(500, ApiResponse<List<MatchStatusDto>>.Error("An error occurred while retrieving active match statuses"));
            }
        }

        /// <summary>
        /// Creates a new match status
        /// </summary>
        /// <param name="createDto">Match status creation details</param>
        /// <returns>Created match status details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<MatchStatusDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<MatchStatusDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<MatchStatusDto>>> CreateMatchStatus([FromBody] CreateMatchStatusDto createDto)
        {
            try
            {
                var command = new CreateMatchStatusCommand { MatchStatus = createDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<MatchStatusDto>.Error(result.ErrorMessage ?? "Failed to create match status", result.Errors));
                }

                return CreatedAtAction(
                    nameof(GetMatchStatusById),
                    new { id = result.Data!.Id },
                    ApiResponse<MatchStatusDto>.SuccessResponse(result.Data, "Match status created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating match status");
                return StatusCode(500, ApiResponse<MatchStatusDto>.Error("An error occurred while creating the match status"));
            }
        }

        /// <summary>
        /// Updates an existing match status
        /// </summary>
        /// <param name="id">Match status ID</param>
        /// <param name="updateDto">Match status update details</param>
        /// <returns>Updated match status details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<MatchStatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<MatchStatusDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<MatchStatusDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<MatchStatusDto>>> UpdateMatchStatus(string id, [FromBody] UpdateMatchStatusDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return BadRequest(ApiResponse<MatchStatusDto>.Error("Match status ID mismatch"));
                }

                var command = new UpdateMatchStatusCommand { MatchStatus = updateDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage?.Contains("not found") == true)
                    {
                        return NotFound(ApiResponse<MatchStatusDto>.Error(result.ErrorMessage));
                    }
                    return BadRequest(ApiResponse<MatchStatusDto>.Error(result.ErrorMessage ?? "Failed to update match status", result.Errors));
                }

                return Ok(ApiResponse<MatchStatusDto>.SuccessResponse(result.Data!, "Match status updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating match status {Id}", id);
                return StatusCode(500, ApiResponse<MatchStatusDto>.Error("An error occurred while updating the match status"));
            }
        }

        /// <summary>
        /// Deletes a match status (soft delete)
        /// </summary>
        /// <param name="id">Match status ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteMatchStatus(string id)
        {
            try
            {
                var command = new DeleteMatchStatusCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<bool>.Error(result.ErrorMessage ?? "Match status not found"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Match status deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting match status {Id}", id);
                return StatusCode(500, ApiResponse<bool>.Error("An error occurred while deleting the match status"));
            }
        }
    }
}