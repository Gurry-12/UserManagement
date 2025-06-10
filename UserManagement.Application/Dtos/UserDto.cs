using UserManagement.Shared.Enums;

namespace UserManagement.Application.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Roles Role { get; set; }

    }
}
