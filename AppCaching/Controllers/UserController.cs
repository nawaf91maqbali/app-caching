using AppCaching.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppCaching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// UserController is responsible for handling user-related HTTP requests,
        /// </summary>
        private readonly IUserService _userService;
        public UserController(IUserService userService) {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves all users from the database, caching the result for subsequent requests.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}
