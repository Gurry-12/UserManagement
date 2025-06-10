using UserManagement.Application.Dtos;

namespace UserManagement.Application.Services.AbstractServices
{
    public interface IUserService
    {
        Task AddUserAsync(UserDto user);
        Task<List<UserDto>> GetAllUsers();
        Task<UserDto> GetUserById(int id);
    }
}
