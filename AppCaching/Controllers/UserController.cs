using AppCaching.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
                //Setup a stopwatch to measure the elapsed time for the operation
                var stopwatch = Stopwatch.StartNew();

                //Retrieve all users using the UserService
                var users = await _userService.GetAllUsersAsync();

                // Stop the stopwatch to measure the elapsed time
                stopwatch.Stop();
                // Log the elapsed time for performance monitoring
                Console.WriteLine($"Elapsed Time: {stopwatch.Elapsed.TotalMilliseconds} ms");
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}
