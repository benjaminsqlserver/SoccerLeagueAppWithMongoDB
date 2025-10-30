namespace SoccerLeague.API.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using SoccerLeague.API.Models;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.Season;
    using SoccerLeague.Application.Features.Seasons.Commands.CreateSeason;
    using SoccerLeague.Application.Features.Seasons.Commands.UpdateSeason;
    using SoccerLeague.Application.Features.Seasons.Commands.DeleteSeason;
    using SoccerLeague.Application.Features.Seasons.Queries.GetAllSeasons;
    using SoccerLeague.Application.Features.Seasons.Queries.GetSeasonById;
    using SoccerLeague.Application.Features.Seasons.Queries.GetCurrentSeason;

    /// <summary>
    /// API Controller for managing seasons
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SeasonsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SeasonsController> _logger;

        public SeasonsController(IMediator mediator, ILogger<SeasonsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of all seasons with optional filtering
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of seasons</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SeasonDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SeasonDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PagedResult<SeasonDto>>>> GetAllSeasons([FromQuery] SeasonQueryParameters parameters)
        {
            try
            {
                var query = new GetAllSeasonsQuery { Parameters = parameters };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<PagedResult<SeasonDto>>.Error(result.ErrorMessage ?? "Failed to retrieve seasons"));
                }

                return Ok(ApiResponse<PagedResult<SeasonDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving seasons");
                return StatusCode(500, ApiResponse<PagedResult<SeasonDto>>.Error("An error occurred while retrieving seasons"));
            }
        }

        /// <summary>
        /// Gets a specific season by ID
        /// </summary>
        /// <param name="id">Season ID</param>
        /// <returns>Season details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SeasonDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<SeasonDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<SeasonDto>>> GetSeasonById(string id)
        {
            try
            {
                var query = new GetSeasonByIdQuery { Id = id };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<SeasonDto>.Error(result.ErrorMessage ?? "Season not found"));
                }

                return Ok(ApiResponse<SeasonDto>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving season {Id}", id);
                return StatusCode(500, ApiResponse<SeasonDto>.Error("An error occurred while retrieving the season"));
            }
        }

        /// <summary>
        /// Gets the current active season
        /// </summary>
        /// <returns>Current season details</returns>
        [HttpGet("current")]
        [ProducesResponseType(typeof(ApiResponse<SeasonDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<SeasonDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<SeasonDto>>> GetCurrentSeason()
        {
            try
            {
                var query = new GetCurrentSeasonQuery();
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<SeasonDto>.Error(result.ErrorMessage ?? "No current season found"));
                }

                return Ok(ApiResponse<SeasonDto>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current season");
                return StatusCode(500, ApiResponse<SeasonDto>.Error("An error occurred while retrieving the current season"));
            }
        }

        /// <summary>
        /// Creates a new season
        /// </summary>
        /// <param name="createDto">Season creation details</param>
        /// <returns>Created season details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<SeasonDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<SeasonDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<SeasonDto>>> CreateSeason([FromBody] CreateSeasonDto createDto)
        {
            try
            {
                var command = new CreateSeasonCommand { Season = createDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<SeasonDto>.Error(result.ErrorMessage ?? "Failed to create season", result.Errors));
                }

                return CreatedAtAction(
                    nameof(GetSeasonById),
                    new { id = result.Data!.Id },
                    ApiResponse<SeasonDto>.SuccessResponse(result.Data, "Season created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating season");
                return StatusCode(500, ApiResponse<SeasonDto>.Error("An error occurred while creating the season"));
            }
        }

        /// <summary>
        /// Updates an existing season
        /// </summary>
        /// <param name="id">Season ID</param>
        /// <param name="updateDto">Season update details</param>
        /// <returns>Updated season details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SeasonDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<SeasonDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<SeasonDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<SeasonDto>>> UpdateSeason(string id, [FromBody] UpdateSeasonDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return BadRequest(ApiResponse<SeasonDto>.Error("Season ID mismatch"));
                }

                var command = new UpdateSeasonCommand { Season = updateDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage?.Contains("not found") == true)
                    {
                        return NotFound(ApiResponse<SeasonDto>.Error(result.ErrorMessage));
                    }
                    return BadRequest(ApiResponse<SeasonDto>.Error(result.ErrorMessage ?? "Failed to update season", result.Errors));
                }

                return Ok(ApiResponse<SeasonDto>.SuccessResponse(result.Data!, "Season updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating season {Id}", id);
                return StatusCode(500, ApiResponse<SeasonDto>.Error("An error occurred while updating the season"));
            }
        }

        /// <summary>
        /// Deletes a season (soft delete)
        /// </summary>
        /// <param name="id">Season ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteSeason(string id)
        {
            try
            {
                var command = new DeleteSeasonCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<bool>.Error(result.ErrorMessage ?? "Season not found"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Season deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting season {Id}", id);
                return StatusCode(500, ApiResponse<bool>.Error("An error occurred while deleting the season"));
            }
        }
    }
}