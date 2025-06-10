using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.DBContext;
using UserManagement.Infrastructure.Repository.AbstractRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserManagement.Infrastructure.Repository.ConcreteRepository
{
    /// <summary>
    /// Repository implementation for User entity operations
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly UserManagementContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(UserManagementContext context, ILogger<UserRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

       
        public async Task AddUserAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully added user with ID {UserId}", user.Id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to add user to database");
                throw;
            }
        }

      
        public async Task<List<User>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                    .AsNoTracking()
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} users from database", users.Count);
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all users");
                throw;
            }
        }


        public async Task<User> GetUserById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid user ID: {UserId}", id);
                    return null;
                }

                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    _logger.LogInformation("User not found with ID: {UserId}", id);
                }
                else
                {
                    _logger.LogInformation("Retrieved user with ID: {UserId}", id);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user with ID {UserId}", id);
                throw;
            }
        }

       
        public async Task UpdateUserAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                if (user.Id <= 0)
                {
                    throw new ArgumentException("Invalid user ID", nameof(user));
                }

                var existingUser = await _context.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    throw new InvalidOperationException($"User with ID {user.Id} not found");
                }

                // Update only the properties that can be modified
                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.Role = user.Role;
                // Don't update the Date field as it represents creation date

                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated user with ID {UserId}", user.Id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to update user with ID {UserId}", user?.Id);
                throw;
            }
        }
    }
}

