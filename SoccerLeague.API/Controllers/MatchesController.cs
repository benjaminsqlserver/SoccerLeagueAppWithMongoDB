namespace SoccerLeague.API.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using SoccerLeague.API.Models;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.Match;
    using SoccerLeague.Application.Features.Matches.Commands.AddMatchEvent;
    using SoccerLeague.Application.Features.Matches.Commands.CreateMatch;
    using SoccerLeague.Application.Features.Matches.Commands.DeleteMatch;
    using SoccerLeague.Application.Features.Matches.Commands.UpdateMatch;
    using SoccerLeague.Application.Features.Matches.Queries.GetAllMatches;
    using SoccerLeague.Application.Features.Matches.Queries.GetMatchById;
    using SoccerLeague.Application.Features.Matches.Queries.GetMatchesBySeason;
    using SoccerLeague.Application.Features.Matches.Queries.GetMatchesByTeam;
    using SoccerLeague.Application.Features.Matches.Queries.GetRecentMatches;
    using SoccerLeague.Application.Features.Matches.Queries.GetUpcomingMatches;

    /// <summary>
    /// API Controller for managing soccer matches and match events
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MatchesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MatchesController> _logger;

        public MatchesController(IMediator mediator, ILogger<MatchesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of matches with optional filtering
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of matches</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<MatchDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<MatchDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PagedResult<MatchDto>>>> GetMatches([FromQuery] MatchQueryParameters parameters)
        {
            try
            {
                var query = new GetAllMatchesQuery { Parameters = parameters };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<PagedResult<MatchDto>>.Error(result.ErrorMessage ?? "Failed to retrieve matches"));
                }

                return Ok(ApiResponse<PagedResult<MatchDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving matches");
                return StatusCode(500, ApiResponse<PagedResult<MatchDto>>.Error("An error occurred while retrieving matches"));
            }
        }

        /// <summary>
        /// Gets a specific match by ID with full details
        /// </summary>
        /// <param name="id">Match ID</param>
        /// <returns>Match details including events</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<MatchDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<MatchDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<MatchDto>>> GetMatchById(string id)
        {
            try
            {
                var query = new GetMatchByIdQuery { MatchId = id };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<MatchDto>.Error(result.ErrorMessage ?? "Match not found"));
                }

                return Ok(ApiResponse<MatchDto>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving match {MatchId}", id);
                return StatusCode(500, ApiResponse<MatchDto>.Error("An error occurred while retrieving the match"));
            }
        }

        /// <summary>
        /// Gets all matches for a specific season
        /// </summary>
        /// <param name="seasonId">Season ID</param>
        /// <returns>List of matches for the season</returns>
        [HttpGet("season/{seasonId}")]
        [ProducesResponseType(typeof(ApiResponse<List<MatchDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<MatchDto>>>> GetMatchesBySeason(string seasonId)
        {
            try
            {
                var query = new GetMatchesBySeasonQuery { SeasonId = seasonId };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<MatchDto>>.Error(result.ErrorMessage ?? "Failed to retrieve matches"));
                }

                return Ok(ApiResponse<List<MatchDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving matches for season {SeasonId}", seasonId);
                return StatusCode(500, ApiResponse<List<MatchDto>>.Error("An error occurred while retrieving matches"));
            }
        }

        /// <summary>
        /// Gets all matches for a specific team
        /// </summary>
        /// <param name="teamId">Team ID</param>
        /// <returns>List of matches involving the team</returns>
        [HttpGet("team/{teamId}")]
        [ProducesResponseType(typeof(ApiResponse<List<MatchDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<MatchDto>>>> GetMatchesByTeam(string teamId)
        {
            try
            {
                var query = new GetMatchesByTeamQuery { TeamId = teamId };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<MatchDto>>.Error(result.ErrorMessage ?? "Failed to retrieve matches"));
                }

                return Ok(ApiResponse<List<MatchDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving matches for team {TeamId}", teamId);
                return StatusCode(500, ApiResponse<List<MatchDto>>.Error("An error occurred while retrieving matches"));
            }
        }

        /// <summary>
        /// Gets upcoming matches
        /// </summary>
        /// <param name="count">Number of matches to retrieve (default: 10)</param>
        /// <returns>List of upcoming matches</returns>
        [HttpGet("upcoming")]
        [ProducesResponseType(typeof(ApiResponse<List<MatchDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<MatchDto>>>> GetUpcomingMatches([FromQuery] int count = 10)
        {
            try
            {
                var query = new GetUpcomingMatchesQuery { Count = count };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<MatchDto>>.Error(result.ErrorMessage ?? "Failed to retrieve upcoming matches"));
                }

                return Ok(ApiResponse<List<MatchDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving upcoming matches");
                return StatusCode(500, ApiResponse<List<MatchDto>>.Error("An error occurred while retrieving upcoming matches"));
            }
        }

        /// <summary>
        /// Gets recent completed matches
        /// </summary>
        /// <param name="count">Number of matches to retrieve (default: 10)</param>
        /// <returns>List of recent matches</returns>
        [HttpGet("recent")]
        [ProducesResponseType(typeof(ApiResponse<List<MatchDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<MatchDto>>>> GetRecentMatches([FromQuery] int count = 10)
        {
            try
            {
                var query = new GetRecentMatchesQuery { Count = count };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<MatchDto>>.Error(result.ErrorMessage ?? "Failed to retrieve recent matches"));
                }

                return Ok(ApiResponse<List<MatchDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent matches");
                return StatusCode(500, ApiResponse<List<MatchDto>>.Error("An error occurred while retrieving recent matches"));
            }
        }

        /// <summary>
        /// Creates a new match
        /// </summary>
        /// <param name="createMatchDto">Match creation details</param>
        /// <returns>Created match details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<MatchDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<MatchDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<MatchDto>>> CreateMatch([FromBody] CreateMatchDto createMatchDto)
        {
            try
            {
                var command = new CreateMatchCommand { Match = createMatchDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<MatchDto>.Error(result.ErrorMessage ?? "Failed to create match", result.Errors));
                }

                return CreatedAtAction(
                    nameof(GetMatchById),
                    new { id = result.Data!.Id },
                    ApiResponse<MatchDto>.SuccessResponse(result.Data, "Match created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating match");
                return StatusCode(500, ApiResponse<MatchDto>.Error("An error occurred while creating the match"));
            }
        }

        /// <summary>
        /// Updates an existing match
        /// </summary>
        /// <param name="id">Match ID</param>
        /// <param name="updateMatchDto">Match update details</param>
        /// <returns>Updated match details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<MatchDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<MatchDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<MatchDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<MatchDto>>> UpdateMatch(string id, [FromBody] UpdateMatchDto updateMatchDto)
        {
            try
            {
                if (id != updateMatchDto.Id)
                {
                    return BadRequest(ApiResponse<MatchDto>.Error("Match ID mismatch"));
                }

                var command = new UpdateMatchCommand { Match = updateMatchDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage?.Contains("not found") == true)
                    {
                        return NotFound(ApiResponse<MatchDto>.Error(result.ErrorMessage));
                    }
                    return BadRequest(ApiResponse<MatchDto>.Error(result.ErrorMessage ?? "Failed to update match", result.Errors));
                }

                return Ok(ApiResponse<MatchDto>.SuccessResponse(result.Data!, "Match updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating match {MatchId}", id);
                return StatusCode(500, ApiResponse<MatchDto>.Error("An error occurred while updating the match"));
            }
        }

        /// <summary>
        /// Deletes a match (soft delete)
        /// </summary>
        /// <param name="id">Match ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteMatch(string id)
        {
            try
            {
                var command = new DeleteMatchCommand { MatchId = id };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<bool>.Error(result.ErrorMessage ?? "Match not found"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Match deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting match {MatchId}", id);
                return StatusCode(500, ApiResponse<bool>.Error("An error occurred while deleting the match"));
            }
        }

        /// <summary>
        /// Adds an event to a match (goal, card, substitution, etc.)
        /// </summary>
        /// <param name="matchId">Match ID</param>
        /// <param name="matchEventDto">Match event details</param>
        /// <returns>Success status</returns>
        [HttpPost("{matchId}/events")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> AddMatchEvent(string matchId, [FromBody] CreateMatchEventDto matchEventDto)
        {
            try
            {
                if (matchId != matchEventDto.MatchId)
                {
                    return BadRequest(ApiResponse<bool>.Error("Match ID mismatch"));
                }

                var command = new AddMatchEventCommand { MatchEvent = matchEventDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.Error(result.ErrorMessage ?? "Failed to add match event", result.Errors));
                }

                return CreatedAtAction(
                    nameof(GetMatchById),
                    new { id = matchId },
                    ApiResponse<bool>.SuccessResponse(true, "Match event added successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding event to match {MatchId}", matchId);
                return StatusCode(500, ApiResponse<bool>.Error("An error occurred while adding the match event"));
            }
        }
    }
}
