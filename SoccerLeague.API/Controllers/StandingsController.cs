namespace SoccerLeague.API.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using SoccerLeague.API.Models;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.Standing;
    using SoccerLeague.Application.Features.Standings.Commands.CreateStanding;
    using SoccerLeague.Application.Features.Standings.Commands.UpdateStanding;
    using SoccerLeague.Application.Features.Standings.Commands.DeleteStanding;
    using SoccerLeague.Application.Features.Standings.Queries.GetAllStandings;
    using SoccerLeague.Application.Features.Standings.Queries.GetStandingById;
    using SoccerLeague.Application.Features.Standings.Queries.GetStandingsBySeason;

    /// <summary>
    /// API Controller for managing league standings/tables
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class StandingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<StandingsController> _logger;

        public StandingsController(IMediator mediator, ILogger<StandingsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of all standings with optional filtering
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of standings</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<StandingDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<StandingDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PagedResult<StandingDto>>>> GetAllStandings([FromQuery] StandingQueryParameters parameters)
        {
            try
            {
                var query = new GetAllStandingsQuery { Parameters = parameters };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<PagedResult<StandingDto>>.Error(result.ErrorMessage ?? "Failed to retrieve standings"));
                }

                return Ok(ApiResponse<PagedResult<StandingDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving standings");
                return StatusCode(500, ApiResponse<PagedResult<StandingDto>>.Error("An error occurred while retrieving standings"));
            }
        }

        /// <summary>
        /// Gets a specific standing by ID
        /// </summary>
        /// <param name="id">Standing ID</param>
        /// <returns>Standing details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<StandingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<StandingDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<StandingDto>>> GetStandingById(string id)
        {
            try
            {
                var query = new GetStandingByIdQuery { Id = id };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<StandingDto>.Error(result.ErrorMessage ?? "Standing not found"));
                }

                return Ok(ApiResponse<StandingDto>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving standing {Id}", id);
                return StatusCode(500, ApiResponse<StandingDto>.Error("An error occurred while retrieving the standing"));
            }
        }

        /// <summary>
        /// Gets all standings for a specific season (league table)
        /// </summary>
        /// <param name="seasonId">Season ID</param>
        /// <returns>List of standings ordered by position</returns>
        [HttpGet("season/{seasonId}")]
        [ProducesResponseType(typeof(ApiResponse<List<StandingDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<StandingDto>>>> GetStandingsBySeason(string seasonId)
        {
            try
            {
                var query = new GetStandingsBySeasonQuery { SeasonId = seasonId };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<StandingDto>>.Error(result.ErrorMessage ?? "Failed to retrieve standings"));
                }

                return Ok(ApiResponse<List<StandingDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving standings for season {SeasonId}", seasonId);
                return StatusCode(500, ApiResponse<List<StandingDto>>.Error("An error occurred while retrieving standings"));
            }
        }

        /// <summary>
        /// Creates a new standing entry
        /// </summary>
        /// <param name="createDto">Standing creation details</param>
        /// <returns>Created standing details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<StandingDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<StandingDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<StandingDto>>> CreateStanding([FromBody] CreateStandingDto createDto)
        {
            try
            {
                var command = new CreateStandingCommand { Standing = createDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<StandingDto>.Error(result.ErrorMessage ?? "Failed to create standing", result.Errors));
                }

                return CreatedAtAction(
                    nameof(GetStandingById),
                    new { id = result.Data!.Id },
                    ApiResponse<StandingDto>.SuccessResponse(result.Data, "Standing created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating standing");
                return StatusCode(500, ApiResponse<StandingDto>.Error("An error occurred while creating the standing"));
            }
        }

        /// <summary>
        /// Updates an existing standing
        /// </summary>
        /// <param name="id">Standing ID</param>
        /// <param name="updateDto">Standing update details</param>
        /// <returns>Updated standing details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<StandingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<StandingDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<StandingDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<StandingDto>>> UpdateStanding(string id, [FromBody] UpdateStandingDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return BadRequest(ApiResponse<StandingDto>.Error("Standing ID mismatch"));
                }

                var command = new UpdateStandingCommand { Standing = updateDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage?.Contains("not found") == true)
                    {
                        return NotFound(ApiResponse<StandingDto>.Error(result.ErrorMessage));
                    }
                    return BadRequest(ApiResponse<StandingDto>.Error(result.ErrorMessage ?? "Failed to update standing", result.Errors));
                }

                return Ok(ApiResponse<StandingDto>.SuccessResponse(result.Data!, "Standing updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating standing {Id}", id);
                return StatusCode(500, ApiResponse<StandingDto>.Error("An error occurred while updating the standing"));
            }
        }

        /// <summary>
        /// Deletes a standing (soft delete)
        /// </summary>
        /// <param name="id">Standing ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteStanding(string id)
        {
            try
            {
                var command = new DeleteStandingCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<bool>.Error(result.ErrorMessage ?? "Standing not found"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Standing deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting standing {Id}", id);
                return StatusCode(500, ApiResponse<bool>.Error("An error occurred while deleting the standing"));
            }
        }
    }
}