namespace SoccerLeague.API.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using SoccerLeague.API.Models;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.Player;
    using SoccerLeague.Application.Features.Players.Commands.CreatePlayer;
    using SoccerLeague.Application.Features.Players.Commands.UpdatePlayer;
    using SoccerLeague.Application.Features.Players.Commands.DeletePlayer;
    using SoccerLeague.Application.Features.Players.Queries.GetAllPlayers;
    using SoccerLeague.Application.Features.Players.Queries.GetPlayerById;
    using SoccerLeague.Application.Features.Players.Queries.GetPlayersByTeam;
    using SoccerLeague.Application.Features.Players.Queries.GetTopScorers;

    /// <summary>
    /// API Controller for managing players
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PlayersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PlayersController> _logger;

        public PlayersController(IMediator mediator, ILogger<PlayersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of all players with optional filtering
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of players</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PlayerDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PlayerDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PagedResult<PlayerDto>>>> GetAllPlayers([FromQuery] PlayerQueryParameters parameters)
        {
            try
            {
                var query = new GetAllPlayersQuery { Parameters = parameters };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<PagedResult<PlayerDto>>.Error(result.ErrorMessage ?? "Failed to retrieve players"));
                }

                return Ok(ApiResponse<PagedResult<PlayerDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving players");
                return StatusCode(500, ApiResponse<PagedResult<PlayerDto>>.Error("An error occurred while retrieving players"));
            }
        }

        /// <summary>
        /// Gets a specific player by ID
        /// </summary>
        /// <param name="id">Player ID</param>
        /// <returns>Player details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PlayerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PlayerDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PlayerDto>>> GetPlayerById(string id)
        {
            try
            {
                var query = new GetPlayerByIdQuery { Id = id };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<PlayerDto>.Error(result.ErrorMessage ?? "Player not found"));
                }

                return Ok(ApiResponse<PlayerDto>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving player {Id}", id);
                return StatusCode(500, ApiResponse<PlayerDto>.Error("An error occurred while retrieving the player"));
            }
        }

        /// <summary>
        /// Gets all players for a specific team
        /// </summary>
        /// <param name="teamId">Team ID</param>
        /// <returns>List of players for the team</returns>
        [HttpGet("team/{teamId}")]
        [ProducesResponseType(typeof(ApiResponse<List<PlayerDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<PlayerDto>>>> GetPlayersByTeam(string teamId)
        {
            try
            {
                var query = new GetPlayersByTeamQuery { TeamId = teamId };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<PlayerDto>>.Error(result.ErrorMessage ?? "Failed to retrieve players"));
                }

                return Ok(ApiResponse<List<PlayerDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving players for team {TeamId}", teamId);
                return StatusCode(500, ApiResponse<List<PlayerDto>>.Error("An error occurred while retrieving players"));
            }
        }

        /// <summary>
        /// Gets the top scorers
        /// </summary>
        /// <param name="count">Number of players to retrieve (default: 10)</param>
        /// <returns>List of top scorers</returns>
        [HttpGet("top-scorers")]
        [ProducesResponseType(typeof(ApiResponse<List<PlayerDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<PlayerDto>>>> GetTopScorers([FromQuery] int count = 10)
        {
            try
            {
                var query = new GetTopScorersQuery { Count = count };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<PlayerDto>>.Error(result.ErrorMessage ?? "Failed to retrieve top scorers"));
                }

                return Ok(ApiResponse<List<PlayerDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top scorers");
                return StatusCode(500, ApiResponse<List<PlayerDto>>.Error("An error occurred while retrieving top scorers"));
            }
        }

        /// <summary>
        /// Creates a new player
        /// </summary>
        /// <param name="createDto">Player creation details</param>
        /// <returns>Created player details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PlayerDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<PlayerDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PlayerDto>>> CreatePlayer([FromBody] CreatePlayerDto createDto)
        {
            try
            {
                var command = new CreatePlayerCommand { Player = createDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<PlayerDto>.Error(result.ErrorMessage ?? "Failed to create player", result.Errors));
                }

                return CreatedAtAction(
                    nameof(GetPlayerById),
                    new { id = result.Data!.Id },
                    ApiResponse<PlayerDto>.SuccessResponse(result.Data, "Player created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating player");
                return StatusCode(500, ApiResponse<PlayerDto>.Error("An error occurred while creating the player"));
            }
        }

        /// <summary>
        /// Updates an existing player
        /// </summary>
        /// <param name="id">Player ID</param>
        /// <param name="updateDto">Player update details</param>
        /// <returns>Updated player details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PlayerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PlayerDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<PlayerDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PlayerDto>>> UpdatePlayer(string id, [FromBody] UpdatePlayerDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return BadRequest(ApiResponse<PlayerDto>.Error("Player ID mismatch"));
                }

                var command = new UpdatePlayerCommand { Player = updateDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage?.Contains("not found") == true)
                    {
                        return NotFound(ApiResponse<PlayerDto>.Error(result.ErrorMessage));
                    }
                    return BadRequest(ApiResponse<PlayerDto>.Error(result.ErrorMessage ?? "Failed to update player", result.Errors));
                }

                return Ok(ApiResponse<PlayerDto>.SuccessResponse(result.Data!, "Player updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating player {Id}", id);
                return StatusCode(500, ApiResponse<PlayerDto>.Error("An error occurred while updating the player"));
            }
        }

        /// <summary>
        /// Deletes a player (soft delete)
        /// </summary>
        /// <param name="id">Player ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeletePlayer(string id)
        {
            try
            {
                var command = new DeletePlayerCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<bool>.Error(result.ErrorMessage ?? "Player not found"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Player deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting player {Id}", id);
                return StatusCode(500, ApiResponse<bool>.Error("An error occurred while deleting the player"));
            }
        }
    }
}