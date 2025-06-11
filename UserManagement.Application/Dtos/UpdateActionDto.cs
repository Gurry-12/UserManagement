using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Shared.Enums;

namespace UserManagement.Application.Dtos
{
    public class UpdateActionDto
    {
        public int Id { get; set; }

        public Actions Action { get; set; } // Assuming Actions is an enum defined elsewhere
    }
}
