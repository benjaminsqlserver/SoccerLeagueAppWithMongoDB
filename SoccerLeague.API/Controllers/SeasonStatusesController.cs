namespace SoccerLeague.API.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using SoccerLeague.API.Models;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.SeasonStatus;
    using SoccerLeague.Application.Features.SeasonStatuses.Commands.CreateSeasonStatus;
    using SoccerLeague.Application.Features.SeasonStatuses.Commands.UpdateSeasonStatus;
    using SoccerLeague.Application.Features.SeasonStatuses.Commands.DeleteSeasonStatus;
    using SoccerLeague.Application.Features.SeasonStatuses.Queries.GetAllSeasonStatuses;
    using SoccerLeague.Application.Features.SeasonStatuses.Queries.GetSeasonStatusById;
    using SoccerLeague.Application.Features.SeasonStatuses.Queries.GetActiveSeasonStatuses;

    /// <summary>
    /// API Controller for managing season statuses (Not Started, In Progress, Completed, Cancelled)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SeasonStatusesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SeasonStatusesController> _logger;

        public SeasonStatusesController(IMediator mediator, ILogger<SeasonStatusesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of all season statuses
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of season statuses</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SeasonStatusDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SeasonStatusDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PagedResult<SeasonStatusDto>>>> GetAllSeasonStatuses([FromQuery] QueryParameters parameters)
        {
            try
            {
                var query = new GetAllSeasonStatusesQuery { Parameters = parameters };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<PagedResult<SeasonStatusDto>>.Error(result.ErrorMessage ?? "Failed to retrieve season statuses"));
                }

                return Ok(ApiResponse<PagedResult<SeasonStatusDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving season statuses");
                return StatusCode(500, ApiResponse<PagedResult<SeasonStatusDto>>.Error("An error occurred while retrieving season statuses"));
            }
        }

        /// <summary>
        /// Gets a specific season status by ID
        /// </summary>
        /// <param name="id">Season status ID</param>
        /// <returns>Season status details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SeasonStatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<SeasonStatusDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<SeasonStatusDto>>> GetSeasonStatusById(string id)
        {
            try
            {
                var query = new GetSeasonStatusByIdQuery { Id = id };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<SeasonStatusDto>.Error(result.ErrorMessage ?? "Season status not found"));
                }

                return Ok(ApiResponse<SeasonStatusDto>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving season status {Id}", id);
                return StatusCode(500, ApiResponse<SeasonStatusDto>.Error("An error occurred while retrieving the season status"));
            }
        }

        /// <summary>
        /// Gets all active season statuses
        /// </summary>
        /// <returns>List of active season statuses ordered by display order</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<SeasonStatusDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<SeasonStatusDto>>>> GetActiveSeasonStatuses()
        {
            try
            {
                var query = new GetActiveSeasonStatusesQuery();
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<SeasonStatusDto>>.Error(result.ErrorMessage ?? "Failed to retrieve active season statuses"));
                }

                return Ok(ApiResponse<List<SeasonStatusDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active season statuses");
                return StatusCode(500, ApiResponse<List<SeasonStatusDto>>.Error("An error occurred while retrieving active season statuses"));
            }
        }

        /// <summary>
        /// Creates a new season status
        /// </summary>
        /// <param name="createDto">Season status creation details</param>
        /// <returns>Created season status details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<SeasonStatusDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<SeasonStatusDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<SeasonStatusDto>>> CreateSeasonStatus([FromBody] CreateSeasonStatusDto createDto)
        {
            try
            {
                var command = new CreateSeasonStatusCommand { SeasonStatus = createDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<SeasonStatusDto>.Error(result.ErrorMessage ?? "Failed to create season status", result.Errors));
                }

                return CreatedAtAction(
                    nameof(GetSeasonStatusById),
                    new { id = result.Data!.Id },
                    ApiResponse<SeasonStatusDto>.SuccessResponse(result.Data, "Season status created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating season status");
                return StatusCode(500, ApiResponse<SeasonStatusDto>.Error("An error occurred while creating the season status"));
            }
        }

        /// <summary>
        /// Updates an existing season status
        /// </summary>
        /// <param name="id">Season status ID</param>
        /// <param name="updateDto">Season status update details</param>
        /// <returns>Updated season status details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SeasonStatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<SeasonStatusDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<SeasonStatusDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<SeasonStatusDto>>> UpdateSeasonStatus(string id, [FromBody] UpdateSeasonStatusDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return BadRequest(ApiResponse<SeasonStatusDto>.Error("Season status ID mismatch"));
                }

                var command = new UpdateSeasonStatusCommand { SeasonStatus = updateDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage?.Contains("not found") == true)
                    {
                        return NotFound(ApiResponse<SeasonStatusDto>.Error(result.ErrorMessage));
                    }
                    return BadRequest(ApiResponse<SeasonStatusDto>.Error(result.ErrorMessage ?? "Failed to update season status", result.Errors));
                }

                return Ok(ApiResponse<SeasonStatusDto>.SuccessResponse(result.Data!, "Season status updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating season status {Id}", id);
                return StatusCode(500, ApiResponse<SeasonStatusDto>.Error("An error occurred while updating the season status"));
            }
        }

        /// <summary>
        /// Deletes a season status (soft delete)
        /// </summary>
        /// <param name="id">Season status ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteSeasonStatus(string id)
        {
            try
            {
                var command = new DeleteSeasonStatusCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<bool>.Error(result.ErrorMessage ?? "Season status not found"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Season status deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting season status {Id}", id);
                return StatusCode(500, ApiResponse<bool>.Error("An error occurred while deleting the season status"));
            }
        }
    }
}