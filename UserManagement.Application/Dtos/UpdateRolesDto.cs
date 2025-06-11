using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Shared.Enums;

namespace UserManagement.Application.Dtos
{
    public class UpdateRolesDto
    {
        public int Id { get; set; }
        public List<Roles> Role { get; set; }
        
    }
}
