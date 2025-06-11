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
                if (user == null)
                {
                    _logger.LogError("Attempted to create user with null data");
                    throw new ArgumentNullException(nameof(user), "User data cannot be null");
                }
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
                    _logger.LogError("Failed to create user with email {Email}", user.Email);
                    throw new InvalidOperationException("Failed to create user");
                }

                user.Id = userEntity.Id;
                _logger.LogInformation("Successfully created user with ID {UserId}", user.Id);
            }
            catch (Exception ex) when (ex is not ArgumentNullException && ex is not CustomValidationException)
            {
                _logger.LogError(ex, "Error occurred while creating user with email {Email}", user?.Email);
                throw;
            }
        }

       
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();

            if (!users.Any())
                return new List<UserDto>();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = Enum.GetValues(typeof(Roles))
               .Cast<Roles>()
               .Where(r => r != Roles.None && u.Role.HasFlag(r)) // ✅ extract individual roles
               .ToList(),
                Date = u.Date
            }).ToList();
        }


        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = Enum.GetValues(typeof(Roles))
               .Cast<Roles>()
               .Where(r => r != Roles.None && user.Role.HasFlag(r))
               .ToList(),
                Date = user.Date
            };

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

                

                var existingUser = await _userRepository.GetUserByIdAsync(user.Id);
                if (existingUser == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", user.Id);
                    throw new KeyNotFoundException($"User with ID {user.Id} not found");
                }

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
                if (id <= 0)
                {
                    _logger.LogError("Attempted to delete user with invalid ID {UserId}", id);
                    throw new ArgumentException("Invalid user ID", nameof(id));
                }
                var user = await _userRepository.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Attempted to delete non-existing user with ID {UserId}", id);
                    throw new KeyNotFoundException($"User with ID {id} not found");
                }
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
                var users = await _userRepository.GetAllUsersAsync();

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
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto));

                var user = await _userRepository.GetUserByIdAsync(dto.Id);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {dto.Id} not found");

                user.Action = dto.Action;

                await _userRepository.UpdateUserAsync(user);
                _logger.LogInformation("Updated fields for user ID {UserId}", dto.Id);
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
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto));

                var user = await _userRepository.GetUserByIdAsync(dto.Id);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {dto.Id} not found");

                user.Role = dto.Role.Aggregate(Roles.None, (acc, next) => acc | next);

                await _userRepository.UpdateUserAsync(user);
                _logger.LogInformation("Updated role for user ID {UserId}", dto.Id);
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
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto));

                var user = await _userRepository.GetUserByIdAsync(dto.Id);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {dto.Id} not found");

                user.Status = dto.Status;

                await _userRepository.UpdateUserAsync(user);
                _logger.LogInformation("Updated status for user ID {UserId}", dto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for user ID {UserId}", dto?.Id);
                throw;
            }
        }

    }
}
