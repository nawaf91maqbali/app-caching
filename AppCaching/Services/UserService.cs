using AppCaching.Data;
using AppCaching.Models;
using Microsoft.EntityFrameworkCore;

namespace AppCaching.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
    public class UserService : IUserService
    {
        /// <summary>
        /// UserService is responsible for managing user-related operations,
        /// </summary>
        private readonly AppDbContext _context;
        private readonly ICacheService _cacheService;
        public UserService(AppDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Retrieves all users from the database, caching the result for subsequent requests.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = await _cacheService.GetAsync<IEnumerable<User>>("all_users");

            if (users != null)
                return users;

            users = await _context.Users
                .AsNoTracking()
                .ToListAsync();

            await _cacheService.Set("all_users", users);
            
            return users;
        }
    }
}
