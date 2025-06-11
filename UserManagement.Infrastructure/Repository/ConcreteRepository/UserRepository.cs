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

      
        public async Task<List<User>> GetAllUsersAsync()
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


        public async Task<User> GetUserByIdAsync(int id)
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
                    throw new ArgumentNullException(nameof(user));

                if (user.Id <= 0)
                    throw new ArgumentException("Invalid user ID", nameof(user));

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated user with ID {UserId}", user.Id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to update user with ID {UserId}", user?.Id);
                throw;
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Attempted to delete non-existing user with ID {UserId}", id);
                    return;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted user with ID {UserId}", id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to delete user with ID {UserId}", id);
                throw;
            }
        }

    }
}

