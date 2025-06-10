using UserManagement.Shared.Enums;

namespace UserManagement.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Roles Role { get; set; }
        public string Email { get; set; }
        public Status Status { get; set; }
        public DateTime Date { get; set; }
        public Actions Action { get; set; }

    }
}
