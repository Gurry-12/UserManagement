using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Services.AbstractServices;

namespace UserManagement.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _user;

        public UserController(IUserService user)
        {
            _user = user;
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var user = await _user.GetAllUsers();
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);

        }
    }
}
