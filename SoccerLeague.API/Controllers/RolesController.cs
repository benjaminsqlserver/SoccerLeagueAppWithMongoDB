using MediatR;
using Microsoft.AspNetCore.Mvc;
using SoccerLeague.API.Models;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Role;
using SoccerLeague.Application.Features.Roles.Commands.CreateRole;
using SoccerLeague.Application.Features.Roles.Commands.UpdateRole;
using SoccerLeague.Application.Features.Roles.Commands.DeleteRole;
using SoccerLeague.Application.Features.Roles.Queries.GetAllRoles;
using SoccerLeague.Application.Features.Roles.Queries.GetRoleById;
using SoccerLeague.Application.Features.Roles.Queries.GetActiveRoles;

namespace SoccerLeague.API.Controllers
{
    /// <summary>
    /// API Controller for managing roles
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IMediator mediator, ILogger<RolesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of all roles with optional filtering
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of roles</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<RoleDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<RoleDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PagedResult<RoleDto>>>> GetAllRoles([FromQuery] RoleQueryParameters parameters)
        {
            try
            {
                var query = new GetAllRolesQuery { Parameters = parameters };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<PagedResult<RoleDto>>.Error(result.ErrorMessage ?? "Failed to retrieve roles"));
                }

                return Ok(ApiResponse<PagedResult<RoleDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles");
                return StatusCode(500, ApiResponse<PagedResult<RoleDto>>.Error("An error occurred while retrieving roles"));
            }
        }

        /// <summary>
        /// Gets a specific role by ID
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Role details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<RoleDto>>> GetRoleById(string id)
        {
            try
            {
                var query = new GetRoleByIdQuery { Id = id };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<RoleDto>.Error(result.ErrorMessage ?? "Role not found"));
                }

                return Ok(ApiResponse<RoleDto>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role {Id}", id);
                return StatusCode(500, ApiResponse<RoleDto>.Error("An error occurred while retrieving the role"));
            }
        }

        /// <summary>
        /// Gets all active roles
        /// </summary>
        /// <returns>List of active roles</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<RoleDto>>>> GetActiveRoles()
        {
            try
            {
                var query = new GetActiveRolesQuery();
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<RoleDto>>.Error(result.ErrorMessage ?? "Failed to retrieve active roles"));
                }

                return Ok(ApiResponse<List<RoleDto>>.SuccessResponse(result.Data!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active roles");
                return StatusCode(500, ApiResponse<List<RoleDto>>.Error("An error occurred while retrieving active roles"));
            }
        }

        /// <summary>
        /// Creates a new role
        /// </summary>
        /// <param name="createDto">Role creation details</param>
        /// <returns>Created role details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<RoleDto>>> CreateRole([FromBody] CreateRoleDto createDto)
        {
            try
            {
                var command = new CreateRoleCommand { Role = createDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<RoleDto>.Error(result.ErrorMessage ?? "Failed to create role", result.Errors));
                }

                return CreatedAtAction(
                    nameof(GetRoleById),
                    new { id = result.Data!.Id },
                    ApiResponse<RoleDto>.SuccessResponse(result.Data, "Role created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return StatusCode(500, ApiResponse<RoleDto>.Error("An error occurred while creating the role"));
            }
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <param name="updateDto">Role update details</param>
        /// <returns>Updated role details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<RoleDto>>> UpdateRole(string id, [FromBody] UpdateRoleDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return BadRequest(ApiResponse<RoleDto>.Error("Role ID mismatch"));
                }

                var command = new UpdateRoleCommand { Role = updateDto };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage?.Contains("not found") == true)
                    {
                        return NotFound(ApiResponse<RoleDto>.Error(result.ErrorMessage));
                    }

                    return BadRequest(ApiResponse<RoleDto>.Error(result.ErrorMessage ?? "Failed to update role", result.Errors));
                }

                return Ok(ApiResponse<RoleDto>.SuccessResponse(result.Data!, "Role updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {Id}", id);
                return StatusCode(500, ApiResponse<RoleDto>.Error("An error occurred while updating the role"));
            }
        }

        /// <summary>
        /// Deletes a role (soft delete)
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteRole(string id)
        {
            try
            {
                var command = new DeleteRoleCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage?.Contains("not found") == true)
                    {
                        return NotFound(ApiResponse<bool>.Error(result.ErrorMessage));
                    }

                    return BadRequest(ApiResponse<bool>.Error(result.ErrorMessage ?? "Failed to delete role"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Role deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {Id}", id);
                return StatusCode(500, ApiResponse<bool>.Error("An error occurred while deleting the role"));
            }
        }
    }
}
