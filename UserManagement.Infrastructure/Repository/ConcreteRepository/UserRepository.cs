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
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

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

               return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all users");
                throw;
            }
        }


        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
               return await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);
               
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
                 _context.Users.Update(user);
                await _context.SaveChangesAsync();

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
                if (user is null)
                {
                    _logger.LogWarning("Attempted to delete non-existent user with ID {UserId}", id);
                    return;
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to delete user with ID {UserId}", id);
                throw;
            }
        }

    }
}

