namespace SoccerLeague.API.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using SoccerLeague.API.Models;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.Team;
    using SoccerLeague.Application.Features.Teams.Commands.CreateTeam;
    using SoccerLeague.Application.Features.Teams.Commands.UpdateTeam;
    using SoccerLeague.Application.Features.Teams.Commands.DeleteTeam;
    using SoccerLeague.Application.Features.Teams.Queries.GetAllTeams;
    using SoccerLeague.Application.Features.Teams.Queries.GetTeamById;
    using SoccerLeague.Application.Features.Teams.Queries.GetActiveTeams;
    using SoccerLeague.Application.Features.Teams.Queries.GetTopTeams;

    /// <summary>
    /// API Controller for managing teams
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TeamsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TeamsController> _logger;

        public TeamsController(IMediator mediator, ILogger<TeamsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of all teams with optional filtering
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of teams</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<TeamDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<TeamDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PagedResult<TeamDto>>>> GetAllTeams([FromQuery] TeamQueryParameters parameters)
        {
            try
            {
                var query = new GetAllTeamsQuery { Parameters = parameters };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<PagedResult<TeamDto>>.Error(result.ErrorMessage ?? "Failed to retrieve teams"));
                }

                return Ok(ApiResponse<PagedResult<TeamDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving teams");
                return StatusCode(500, ApiResponse<PagedResult<TeamDto>>.Error("An error occurred while retrieving teams"));
            }
        }

        /// <summary>
        /// Gets a specific team by ID
        /// </summary>
        /// <param name="id">Team ID</param>
        /// <returns>Team details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TeamDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<TeamDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<TeamDto>>> GetTeamById(string id)
        {
            try
            {
                var query = new GetTeamByIdQuery { Id = id };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<TeamDto>.Error(result.ErrorMessage ?? "Team not found"));
                }

                return Ok(ApiResponse<TeamDto>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving team {Id}", id);
                return StatusCode(500, ApiResponse<TeamDto>.Error("An error occurred while retrieving the team"));
            }
        }

        /// <summary>
        /// Gets all active teams
        /// </summary>
        /// <returns>List of active teams</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<TeamDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<TeamDto>>>> GetActiveTeams()
        {
            try
            {
                var query = new GetActiveTeamsQuery();
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<TeamDto>>.Error(result.ErrorMessage ?? "Failed to retrieve active teams"));
                }

                return Ok(ApiResponse<List<TeamDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active teams");
                return StatusCode(500, ApiResponse<List<TeamDto>>.Error("An error occurred while retrieving active teams"));
            }
        }

        /// <summary>
        /// Gets the top teams by points
        /// </summary>
        /// <param name="count">Number of teams to retrieve (default: 10)</param>
        /// <returns>List of top teams</returns>
        [HttpGet("top")]
        [ProducesResponseType(typeof(ApiResponse<List<TeamDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<TeamDto>>>> GetTopTeams([FromQuery] int count = 10)
        {
            try
            {
                var query = new GetTopTeamsQuery { Count = count };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<TeamDto>>.Error(result.ErrorMessage ?? "Failed to retrieve top teams"));
                }

                return Ok(ApiResponse<List<TeamDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top teams");
                return StatusCode(500, ApiResponse<List<TeamDto>>.Error("An error occurred while retrieving top teams"));
            }
        }

        /// <summary>
        /// Creates a new team
        /// </summary>
        /// <param name="createDto">Team creation details</param>
        /// <returns>Created team details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<TeamDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<TeamDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<TeamDto>>> CreateTeam([FromBody] CreateTeamDto createDto)
        {
            try
            {
                var command = new CreateTeamCommand { Team = createDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<TeamDto>.Error(result.ErrorMessage ?? "Failed to create team", result.Errors));
                }

                return CreatedAtAction(
                    nameof(GetTeamById),
                    new { id = result.Data!.Id },
                    ApiResponse<TeamDto>.SuccessResponse(result.Data, "Team created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating team");
                return StatusCode(500, ApiResponse<TeamDto>.Error("An error occurred while creating the team"));
            }
        }

        /// <summary>
        /// Updates an existing team
        /// </summary>
        /// <param name="id">Team ID</param>
        /// <param name="updateDto">Team update details</param>
        /// <returns>Updated team details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TeamDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<TeamDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<TeamDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<TeamDto>>> UpdateTeam(string id, [FromBody] UpdateTeamDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return BadRequest(ApiResponse<TeamDto>.Error("Team ID mismatch"));
                }

                var command = new UpdateTeamCommand { Team = updateDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage?.Contains("not found") == true)
                    {
                        return NotFound(ApiResponse<TeamDto>.Error(result.ErrorMessage));
                    }
                    return BadRequest(ApiResponse<TeamDto>.Error(result.ErrorMessage ?? "Failed to update team", result.Errors));
                }

                return Ok(ApiResponse<TeamDto>.SuccessResponse(result.Data!, "Team updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating team {Id}", id);
                return StatusCode(500, ApiResponse<TeamDto>.Error("An error occurred while updating the team"));
            }
        }

        /// <summary>
        /// Deletes a team (soft delete)
        /// </summary>
        /// <param name="id">Team ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTeam(string id)
        {
            try
            {
                var command = new DeleteTeamCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<bool>.Error(result.ErrorMessage ?? "Team not found"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Team deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting team {Id}", id);
                return StatusCode(500, ApiResponse<bool>.Error("An error occurred while deleting the team"));
            }
        }
    }
}