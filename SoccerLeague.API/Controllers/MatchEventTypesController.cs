namespace SoccerLeague.API.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using SoccerLeague.API.Models;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.MatchEventType;
    using SoccerLeague.Application.Features.MatchEventTypes.Commands.CreateMatchEventType;
    using SoccerLeague.Application.Features.MatchEventTypes.Commands.UpdateMatchEventType;
    using SoccerLeague.Application.Features.MatchEventTypes.Commands.DeleteMatchEventType;
    using SoccerLeague.Application.Features.MatchEventTypes.Queries.GetAllMatchEventTypes;
    using SoccerLeague.Application.Features.MatchEventTypes.Queries.GetMatchEventTypeById;
    using SoccerLeague.Application.Features.MatchEventTypes.Queries.GetActiveMatchEventTypes;
    using SoccerLeague.Application.Features.MatchEventTypes.Queries.GetScoreAffectingTypes;
    using SoccerLeague.Application.Features.MatchEventTypes.Queries.GetDisciplineAffectingTypes;

    /// <summary>
    /// API Controller for managing match event types (goals, cards, substitutions, etc.)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MatchEventTypesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MatchEventTypesController> _logger;

        public MatchEventTypesController(IMediator mediator, ILogger<MatchEventTypesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of all match event types
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of match event types</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<MatchEventTypeDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<MatchEventTypeDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PagedResult<MatchEventTypeDto>>>> GetAllMatchEventTypes([FromQuery] QueryParameters parameters)
        {
            try
            {
                var query = new GetAllMatchEventTypesQuery { Parameters = parameters };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<PagedResult<MatchEventTypeDto>>.Error(result.ErrorMessage ?? "Failed to retrieve match event types"));
                }

                return Ok(ApiResponse<PagedResult<MatchEventTypeDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving match event types");
                return StatusCode(500, ApiResponse<PagedResult<MatchEventTypeDto>>.Error("An error occurred while retrieving match event types"));
            }
        }

        /// <summary>
        /// Gets a specific match event type by ID
        /// </summary>
        /// <param name="id">Match event type ID</param>
        /// <returns>Match event type details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<MatchEventTypeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<MatchEventTypeDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<MatchEventTypeDto>>> GetMatchEventTypeById(string id)
        {
            try
            {
                var query = new GetMatchEventTypeByIdQuery { Id = id };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<MatchEventTypeDto>.Error(result.ErrorMessage ?? "Match event type not found"));
                }

                return Ok(ApiResponse<MatchEventTypeDto>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving match event type {Id}", id);
                return StatusCode(500, ApiResponse<MatchEventTypeDto>.Error("An error occurred while retrieving the match event type"));
            }
        }

        /// <summary>
        /// Gets all active match event types
        /// </summary>
        /// <returns>List of active match event types ordered by display order</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<MatchEventTypeDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<MatchEventTypeDto>>>> GetActiveMatchEventTypes()
        {
            try
            {
                var query = new GetActiveMatchEventTypesQuery();
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<MatchEventTypeDto>>.Error(result.ErrorMessage ?? "Failed to retrieve active match event types"));
                }

                return Ok(ApiResponse<List<MatchEventTypeDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active match event types");
                return StatusCode(500, ApiResponse<List<MatchEventTypeDto>>.Error("An error occurred while retrieving active match event types"));
            }
        }

        /// <summary>
        /// Gets all match event types that affect score (goals, penalties, etc.)
        /// </summary>
        /// <returns>List of score-affecting match event types</returns>
        [HttpGet("score-affecting")]
        [ProducesResponseType(typeof(ApiResponse<List<MatchEventTypeDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<MatchEventTypeDto>>>> GetScoreAffectingTypes()
        {
            try
            {
                var query = new GetScoreAffectingTypesQuery();
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<MatchEventTypeDto>>.Error(result.ErrorMessage ?? "Failed to retrieve score-affecting types"));
                }

                return Ok(ApiResponse<List<MatchEventTypeDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving score-affecting match event types");
                return StatusCode(500, ApiResponse<List<MatchEventTypeDto>>.Error("An error occurred while retrieving score-affecting types"));
            }
        }

        /// <summary>
        /// Gets all match event types that affect discipline (yellow/red cards)
        /// </summary>
        /// <returns>List of discipline-affecting match event types</returns>
        [HttpGet("discipline-affecting")]
        [ProducesResponseType(typeof(ApiResponse<List<MatchEventTypeDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<MatchEventTypeDto>>>> GetDisciplineAffectingTypes()
        {
            try
            {
                var query = new GetDisciplineAffectingTypesQuery();
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<MatchEventTypeDto>>.Error(result.ErrorMessage ?? "Failed to retrieve discipline-affecting types"));
                }

                return Ok(ApiResponse<List<MatchEventTypeDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving discipline-affecting match event types");
                return StatusCode(500, ApiResponse<List<MatchEventTypeDto>>.Error("An error occurred while retrieving discipline-affecting types"));
            }
        }

        /// <summary>
        /// Creates a new match event type
        /// </summary>
        /// <param name="createDto">Match event type creation details</param>
        /// <returns>Created match event type details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<MatchEventTypeDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<MatchEventTypeDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<MatchEventTypeDto>>> CreateMatchEventType([FromBody] CreateMatchEventTypeDto createDto)
        {
            try
            {
                var command = new CreateMatchEventTypeCommand { MatchEventType = createDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<MatchEventTypeDto>.Error(result.ErrorMessage ?? "Failed to create match event type", result.Errors));
                }

                return CreatedAtAction(
                    nameof(GetMatchEventTypeById),
                    new { id = result.Data!.Id },
                    ApiResponse<MatchEventTypeDto>.SuccessResponse(result.Data, "Match event type created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating match event type");
                return StatusCode(500, ApiResponse<MatchEventTypeDto>.Error("An error occurred while creating the match event type"));
            }
        }

        /// <summary>
        /// Updates an existing match event type
        /// </summary>
        /// <param name="id">Match event type ID</param>
        /// <param name="updateDto">Match event type update details</param>
        /// <returns>Updated match event type details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<MatchEventTypeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<MatchEventTypeDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<MatchEventTypeDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<MatchEventTypeDto>>> UpdateMatchEventType(string id, [FromBody] UpdateMatchEventTypeDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return BadRequest(ApiResponse<MatchEventTypeDto>.Error("Match event type ID mismatch"));
                }

                var command = new UpdateMatchEventTypeCommand { MatchEventType = updateDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage?.Contains("not found") == true)
                    {
                        return NotFound(ApiResponse<MatchEventTypeDto>.Error(result.ErrorMessage));
                    }
                    return BadRequest(ApiResponse<MatchEventTypeDto>.Error(result.ErrorMessage ?? "Failed to update match event type", result.Errors));
                }

                return Ok(ApiResponse<MatchEventTypeDto>.SuccessResponse(result.Data!, "Match event type updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating match event type {Id}", id);
                return StatusCode(500, ApiResponse<MatchEventTypeDto>.Error("An error occurred while updating the match event type"));
            }
        }

        /// <summary>
        /// Deletes a match event type (soft delete)
        /// </summary>
        /// <param name="id">Match event type ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteMatchEventType(string id)
        {
            try
            {
                var command = new DeleteMatchEventTypeCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<bool>.Error(result.ErrorMessage ?? "Match event type not found"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Match event type deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting match event type {Id}", id);
                return StatusCode(500, ApiResponse<bool>.Error("An error occurred while deleting the match event type"));
            }
        }
    }
}