using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Repository.AbstractRepository
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int id);
        Task<List<User>> GetAllUsersAsync();
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
    }
}
