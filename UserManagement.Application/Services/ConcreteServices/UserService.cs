using UserManagement.Application.Dtos;
using UserManagement.Application.Services.AbstractServices;
using UserManagement.Infrastructure.Repository.AbstractRepository;
using UserManagement.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        public async Task AddUserAsync(UserDto user)
        {
            try
            {
                // Validate input
                if (user == null)
                {
                    _logger.LogError("Attempted to create user with null data");
                    throw new ArgumentNullException(nameof(user), "User data cannot be null");
                }

                ValidateUserDto(user);

                // Map DTO to entity
                var userEntity = new User
                {
                    Name = user.Name?.Trim(),
                    Role = user.Role,
                    Email = user.Email?.Trim().ToLower(),
                    Date = DateTime.UtcNow
                };

                // Save to database
                await _userRepository.AddUserAsync(userEntity);
                
                if (userEntity.Id <= 0)
                {
                    _logger.LogError("Failed to create user with email {Email}", user.Email);
                    throw new InvalidOperationException("Failed to create user");
                }

                // Update the input DTO with the created user's ID
                user.Id = userEntity.Id;
                _logger.LogInformation("Successfully created user with ID {UserId}", user.Id);
            }
            catch (Exception ex) when (ex is not ArgumentNullException && ex is not ValidationException)
            {
                _logger.LogError(ex, "Error occurred while creating user with email {Email}", user?.Email);
                throw;
            }
        }

        private void ValidateUserDto(UserDto user)
        {
            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(user.Name))
            {
                validationErrors.Add("Name is required");
            }
            else if (user.Name.Length > 100)
            {
                validationErrors.Add("Name cannot exceed 100 characters");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                validationErrors.Add("Email is required");
            }
            else if (!IsValidEmail(user.Email))
            {
                validationErrors.Add("Invalid email format");
            }

            if (validationErrors.Any())
            {
                throw new ValidationException(string.Join(", ", validationErrors));
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();

            var userDtos =  users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Role = u.Role // assuming Role is an enum
            }).ToList();

            return userDtos;
        }

        public async Task<UserDto> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);
             if(user == null)
                return null;

             var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Role = user.Role // assuming Role is an enum
            };
            return userDto;
        }
    }
}
