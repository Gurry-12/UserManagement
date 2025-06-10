using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Repository.AbstractRepository
{
    public interface IUserRepository
    {
        Task<User> GetUserById(int id);
        Task<List<User>> GetAllUsers();

        Task AddUserAsync(User user);

        Task UpdateUserAsync(User user);
    }
}
