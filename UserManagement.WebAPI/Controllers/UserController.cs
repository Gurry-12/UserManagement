using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Dtos;
using UserManagement.Application.Services.AbstractServices;
using UserManagement.Domain.Entities;
using System.Net;
using UserManagement.Shared.Enums;

namespace UserManagement.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing user operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>List of users</returns>
        /// <response code="200">Returns the list of users</response>
        /// <response code="404">If no users are found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("GetAllUsers")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                if (users == null || !users.Any())
                {
                    _logger.LogWarning("No users found");
                    return NotFound("No users found");
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all users");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Gets a user by their ID
        /// </summary>
        /// <param name="id">The ID of the user</param>
        /// <returns>The user details</returns>
        /// <response code="200">Returns the user</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="400">If the ID is invalid</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("GetUserById/{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid user ID");
                }

                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", id);
                    return NotFound($"User with ID {id} not found");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user with ID {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="userDto">The user data</param>
        /// <returns>The created user</returns>
        /// <response code="201">Returns the newly created user</response>
        /// <response code="400">If the user data is invalid</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost("AddUser")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] AddUpdateUserDto userDto)
        {
            try
            {
                if (userDto == null)
                {
                    return BadRequest("User data cannot be null");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _userService.AddUserAsync(userDto);
                return CreatedAtAction(nameof(GetUserById), new { id = userDto.Id }, userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }


        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="userDto">User data to update</param>
        /// <returns>NoContent if successful; otherwise, appropriate error response</returns>
        /// <response code="204">User updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("UpdateUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUser([FromBody] AddUpdateUserDto userDto)
        {
            try
            {
                if (userDto == null || userDto.Id <= 0)
                {
                    return BadRequest("Invalid user data");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                 await _userService.UpdateUserAsync(userDto);
                return NoContent(); // 204
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user with ID {UserId}", userDto?.Id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <returns>NoContent if successful; otherwise, appropriate error response</returns>
        /// <response code="204">User deleted successfully</response>
        /// <response code="400">Invalid user ID</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("DeleteUser/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid user ID");
                }

                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found");
                }

                await _userService.DeleteUserAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Updates the roles of a user
        /// <param name="roles">The roles to assign to the user</param>
        /// <returns>NoContent if successful; otherwise, appropriate error response</returns>
        [HttpPut("UpdateUserRoles")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> UpdateUserRoles([FromBody] UpdateRolesDto roleDto)
        {
            try
            {
                if (roleDto.Id <= 0)
                {
                    return BadRequest("Invalid user ID");
                }
                if (roleDto.Role == null || !roleDto.Role.Any())
                {
                    return BadRequest("Roles cannot be null or empty");
                }
               await _userService.UpdateUserRoleAsync(roleDto);
                
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating roles for user with ID {UserId}", roleDto.Id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update User's Status
        /// <param name="status">The new status to assign to the user</param>
        /// <returns>NoContent if successful; otherwise, appropriate error response</returns>
        [HttpPut("UpdateUserStatus")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUserStatus([FromBody] UpdateStatusDto statusDto)
        {
            try
            {
                if (statusDto.Id <= 0) 
                {
                    return BadRequest("Invalid user ID");
                }
                if (statusDto == null || !Enum.IsDefined(typeof(Status), statusDto.Status))
                {
                    return BadRequest("Invalid user status");
                }
                   await _userService.UpdateUserStatusAsync(statusDto);

                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating status for user with ID {UserId}", statusDto.Id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update User's Action
        /// <param name="Action">The new status to assign to the user</param>
        /// <returns>NoContent if successful; otherwise, appropriate error response</returns>
        [HttpPut("UpdateUserAction")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUserAction([FromBody] UpdateActionDto actionDto)
        {
            try
            {
                if (actionDto.Id <= 0 || actionDto == null)
                {
                    return BadRequest("Invalid user ID");
                }
               
                   await _userService.UpdateUserActionAsync(actionDto);

                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating Action for user with ID {UserId}", actionDto.Id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }


        /// <summary>
        /// Get Summary of the Users 
        /// 
        /// <returns>Summary of users</returns>
        /// 

        [HttpGet("GetUserSummary")]
        [ProducesResponseType(typeof(UserSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<UserSummaryDto>> GetUserSummary()
        {
            try
            {
               
                var summary = await _userService.GetSummaryAsync();
                if (summary == null)
                {
                    _logger.LogWarning("No user summary found");
                    return NotFound("No user summary found");
                }
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user summary");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }

        }
    }

}




