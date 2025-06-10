using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities; // assuming User class is here

namespace UserManagement.Infrastructure.DBContext
{
    public class UserManagementContext : DbContext
    {
        public UserManagementContext(DbContextOptions<UserManagementContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        // Optional: override OnModelCreating if needed
    }
}
