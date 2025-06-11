using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Shared.Enums;

namespace UserManagement.Application.Dtos
{
    public class AddUpdateUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Roles> Role { get; set; } // Assuming Roles is an enum or a list of roles
    }
}
