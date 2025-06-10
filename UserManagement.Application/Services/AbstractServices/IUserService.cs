using UserManagement.Application.Dtos;

namespace UserManagement.Application.Services.AbstractServices
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsers();
    }
}
