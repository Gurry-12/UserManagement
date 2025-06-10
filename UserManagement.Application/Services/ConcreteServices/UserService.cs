using UserManagement.Application.Dtos;
using UserManagement.Application.Services.AbstractServices;
using UserManagement.Infrastructure.Repository.AbstractRepository;

namespace UserManagement.Application.Services.ConcreteServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

    }
}
