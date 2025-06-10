using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Dtos;
using UserManagement.Application.Services.AbstractServices;
using UserManagement.Domain.Entities;
using System.Net;

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
                var users = await _userService.GetAllUsers();
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

                var user = await _userService.GetUserById(id);
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
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserDto userDto)
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
    }
}
