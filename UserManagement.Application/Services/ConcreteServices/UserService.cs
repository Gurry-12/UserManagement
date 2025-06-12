using UserManagement.Application.Dtos;
using UserManagement.Application.Exceptions;
using UserManagement.Application.Services.AbstractServices;
using UserManagement.Infrastructure.Repository.AbstractRepository;
using UserManagement.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UserManagement.Shared.Enums;
using UserManagement.Application.Mappers;

namespace UserManagement.Application.Services.ConcreteServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddUserAsync(AddUpdateUserDto user)
        {
            try
            {
                if (user is null)
                    throw new ArgumentNullException(nameof(user));

                var combinedRoles = user.Role
                                   .Aggregate(Roles.None, (acc, next) => acc | next); // ✅ Combine roles

                var userEntity = new User
                {
                    Name = user.Name.Trim(),
                    Role = combinedRoles, // ✅ Store combined flags
                    Email = user.Email.Trim().ToLowerInvariant(),
                    Date = DateTime.UtcNow
                };


                await _userRepository.AddUserAsync(userEntity);

                if (userEntity.Id <= 0)
                {
                   
                    throw new InvalidOperationException("Failed to create user");
                }

                user.Id = userEntity.Id;
                _logger.LogInformation("Successfully created user with ID {UserId}", user.Id);
            }
            catch (Exception ex) when (ex is not ArgumentNullException && ex is not CustomValidationException)
            {
                _logger.LogError(ex, "Error occurred while creating user with email {userId}", user?.Id);
                throw;
            }
        }

       
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();

            if (!users.Any())
                return new List<UserDto>();

            return users.Select(u => u.ToDto()).ToList();
        }


        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return null;

            return user?.ToDto();

        }

        public async Task UpdateUserAsync(AddUpdateUserDto user)
        {
            try
            {
                if (user == null)
                {
                    _logger.LogError("Attempted to update user with null data");
                    throw new ArgumentNullException(nameof(user), "User data cannot be null");
                }

                // Fetch existing user to ensure it exists
                var existingUser = await FetchUserOrThrowAsync(user.Id);

                // ✅ Only update the intended fields
                existingUser.Name = user.Name.Trim();
                existingUser.Email = user.Email.Trim().ToLowerInvariant();
                existingUser.Role = user.Role.Aggregate(Roles.None, (acc, next) => acc | next);

                await _userRepository.UpdateUserAsync(existingUser);
                _logger.LogInformation("Successfully updated user with ID {UserId}", user.Id);
            }
            catch (Exception ex) when (ex is not ArgumentNullException && ex is not CustomValidationException)
            {
                _logger.LogError(ex, "Error occurred while updating user with ID {UserId}", user?.Id);
                throw;
            }
        }



        public async Task DeleteUserAsync(int id)
        {
            try
            {
                // Validate input

                var user = await FetchUserOrThrowAsync(id);

                // Check if user is already deactivated
                if (user.Status == Status.Deactive)
                {
                    _logger.LogWarning("User with ID {UserId} is already deactivated", id);
                    return; // No need to delete if already deactivated
                }

                // Proceed with deletion
                await _userRepository.DeleteUserAsync(id);
                _logger.LogInformation("Successfully deleted user with ID {UserId}", id);
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not KeyNotFoundException)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID {UserId}", id);
                throw;
            }
        }

        public async Task<UserSummaryDto> GetSummaryAsync()
        {
            try
            {
                // Fetch all users and calculate summary
                var users = await _userRepository.GetAllUsersAsync();

                // If no users found, return empty summary
                if (users == null || !users.Any())
                {
                    _logger.LogInformation("No users found, returning empty summary.");
                    return new UserSummaryDto();
                }

                // Calculate summary counts
                var summary = new UserSummaryDto
                {
                    ClinicianCount = users.Count(u => u.Role.HasFlag(Roles.Clinician)),
                    StaffCount = users.Count(u => u.Role.HasFlag(Roles.Staff)),
                    DeactivatedClinicianCount = users.Count(u =>
                        u.Role.HasFlag(Roles.Clinician) &&
                        u.Status == Status.Deactive)
                };

                _logger.LogInformation("Successfully generated user summary.");
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating user summary.");
                throw;
            }
        }


        public async Task UpdateUserActionAsync(UpdateActionDto dto)
        {
            try
            {
                // Validate input
                ValidateDto(dto);

                if (!Enum.IsDefined(typeof(Actions), dto.Action))
                    throw new ArgumentException("Invalid action value", nameof(dto.Action));

                await UpdateUserFieldAsync(dto.Id, u => u.Action = dto.Action,
       "Updated Action for user ID {UserId}");

                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating action for user ID {UserId}", dto?.Id);
                throw;
            }
        }


        public async Task UpdateUserRoleAsync(UpdateRolesDto dto)
        {
            try
            {
                // Validate input
                ValidateDto(dto);

                if (dto.Role == null || !dto.Role.Any())
                    throw new ArgumentException("Role cannot be null or empty", nameof(dto.Role));

                // Update user role
                await UpdateUserFieldAsync(dto.Id, u => u.Role = dto.Role.Aggregate(Roles.None, (acc, next) => acc | next),
                     "Updated Role for user ID {UserId}");
 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role for user ID {UserId}", dto?.Id);
                throw;
            }
        }


        public async Task UpdateUserStatusAsync(UpdateStatusDto dto)
        {
            try
            {
                // Validate input
                ValidateDto(dto);

                if (!Enum.IsDefined(typeof(Status), dto.Status))
                    throw new ArgumentException("Invalid status value", nameof(dto.Status));
                await UpdateUserFieldAsync(dto.Id, u => u.Status = dto.Status,
       "Updated status for user ID {UserId}");

              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for user ID {UserId}", dto?.Id);
                throw;
            }
        }


        // Helper to fetch or throw common logic
        private async Task<User> FetchUserOrThrowAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user ID", nameof(id));

            var user = await _userRepository.GetUserByIdAsync(id);
            if (user is null)
                throw new KeyNotFoundException($"User {id} not found");
            return user;
        }


        private async Task UpdateUserFieldAsync(int id, Action<User> updateAction, string logMessage)
        {

            var user = await FetchUserOrThrowAsync(id);
            updateAction(user);
            await _userRepository.UpdateUserAsync(user);
            _logger.LogInformation(logMessage, id);
        }


        private static void ValidateDto<T>(T dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
        }


    }
}
