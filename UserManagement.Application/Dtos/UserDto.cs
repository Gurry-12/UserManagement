using System.Text.Json.Serialization;
using UserManagement.Shared.Enums;

namespace UserManagement.Application.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Roles> Role { get; set; }

        public string Email { get; set; }
        public DateTime Date { get; set; }
    }
}
