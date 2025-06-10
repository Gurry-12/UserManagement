using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.DBContext;
using UserManagement.Infrastructure.Repository.AbstractRepository;

namespace UserManagement.Infrastructure.Repository.ConcreteRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManagementContext _context;

        public UserRepository(UserManagementContext context)
        {
            _context = context;
        }

        //AddUserAsync - method
        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public Task UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}

