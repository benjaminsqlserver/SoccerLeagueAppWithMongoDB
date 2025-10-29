namespace SoccerLeague.API.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using SoccerLeague.API.Models;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.PlayerPosition;
    using SoccerLeague.Application.Features.PlayerPositions.Commands.CreatePlayerPosition;
    using SoccerLeague.Application.Features.PlayerPositions.Commands.UpdatePlayerPosition;
    using SoccerLeague.Application.Features.PlayerPositions.Commands.DeletePlayerPosition;
    using SoccerLeague.Application.Features.PlayerPositions.Queries.GetAllPlayerPositions;
    using SoccerLeague.Application.Features.PlayerPositions.Queries.GetPlayerPositionById;
    using SoccerLeague.Application.Features.PlayerPositions.Queries.GetActivePlayerPositions;

    /// <summary>
    /// API Controller for managing player positions (Goalkeeper, Defender, Midfielder, Forward)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PlayerPositionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PlayerPositionsController> _logger;

        public PlayerPositionsController(IMediator mediator, ILogger<PlayerPositionsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of all player positions
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of player positions</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PlayerPositionDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PlayerPositionDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PagedResult<PlayerPositionDto>>>> GetAllPlayerPositions([FromQuery] QueryParameters parameters)
        {
            try
            {
                var query = new GetAllPlayerPositionsQuery { Parameters = parameters };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<PagedResult<PlayerPositionDto>>.Error(result.ErrorMessage ?? "Failed to retrieve player positions"));
                }

                return Ok(ApiResponse<PagedResult<PlayerPositionDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving player positions");
                return StatusCode(500, ApiResponse<PagedResult<PlayerPositionDto>>.Error("An error occurred while retrieving player positions"));
            }
        }

        /// <summary>
        /// Gets a specific player position by ID
        /// </summary>
        /// <param name="id">Player position ID</param>
        /// <returns>Player position details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PlayerPositionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PlayerPositionDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PlayerPositionDto>>> GetPlayerPositionById(string id)
        {
            try
            {
                var query = new GetPlayerPositionByIdQuery { Id = id };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<PlayerPositionDto>.Error(result.ErrorMessage ?? "Player position not found"));
                }

                return Ok(ApiResponse<PlayerPositionDto>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving player position {Id}", id);
                return StatusCode(500, ApiResponse<PlayerPositionDto>.Error("An error occurred while retrieving the player position"));
            }
        }

        /// <summary>
        /// Gets all active player positions
        /// </summary>
        /// <returns>List of active player positions ordered by display order</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<PlayerPositionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<PlayerPositionDto>>>> GetActivePlayerPositions()
        {
            try
            {
                var query = new GetActivePlayerPositionsQuery();
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<PlayerPositionDto>>.Error(result.ErrorMessage ?? "Failed to retrieve active player positions"));
                }

                return Ok(ApiResponse<List<PlayerPositionDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active player positions");
                return StatusCode(500, ApiResponse<List<PlayerPositionDto>>.Error("An error occurred while retrieving active player positions"));
            }
        }

        /// <summary>
        /// Creates a new player position
        /// </summary>
        /// <param name="createDto">Player position creation details</param>
        /// <returns>Created player position details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PlayerPositionDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<PlayerPositionDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PlayerPositionDto>>> CreatePlayerPosition([FromBody] CreatePlayerPositionDto createDto)
        {
            try
            {
                var command = new CreatePlayerPositionCommand { PlayerPosition = createDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<PlayerPositionDto>.Error(result.ErrorMessage ?? "Failed to create player position", result.Errors));
                }

                return CreatedAtAction(
                    nameof(GetPlayerPositionById),
                    new { id = result.Data!.Id },
                    ApiResponse<PlayerPositionDto>.SuccessResponse(result.Data, "Player position created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating player position");
                return StatusCode(500, ApiResponse<PlayerPositionDto>.Error("An error occurred while creating the player position"));
            }
        }

        /// <summary>
        /// Updates an existing player position
        /// </summary>
        /// <param name="id">Player position ID</param>
        /// <param name="updateDto">Player position update details</param>
        /// <returns>Updated player position details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PlayerPositionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PlayerPositionDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<PlayerPositionDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PlayerPositionDto>>> UpdatePlayerPosition(string id, [FromBody] UpdatePlayerPositionDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return BadRequest(ApiResponse<PlayerPositionDto>.Error("Player position ID mismatch"));
                }

                var command = new UpdatePlayerPositionCommand { PlayerPosition = updateDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage?.Contains("not found") == true)
                    {
                        return NotFound(ApiResponse<PlayerPositionDto>.Error(result.ErrorMessage));
                    }
                    return BadRequest(ApiResponse<PlayerPositionDto>.Error(result.ErrorMessage ?? "Failed to update player position", result.Errors));
                }

                return Ok(ApiResponse<PlayerPositionDto>.SuccessResponse(result.Data!, "Player position updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating player position {Id}", id);
                return StatusCode(500, ApiResponse<PlayerPositionDto>.Error("An error occurred while updating the player position"));
            }
        }

        /// <summary>
        /// Deletes a player position (soft delete)
        /// </summary>
        /// <param name="id">Player position ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeletePlayerPosition(string id)
        {
            try
            {
                var command = new DeletePlayerPositionCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<bool>.Error(result.ErrorMessage ?? "Player position not found"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Player position deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting player position {Id}", id);
                return StatusCode(500, ApiResponse<bool>.Error("An error occurred while deleting the player position"));
            }
        }
    }
}