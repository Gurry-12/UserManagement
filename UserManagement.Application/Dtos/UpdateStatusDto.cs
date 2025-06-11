using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Shared.Enums;

namespace UserManagement.Application.Dtos
{
    public class UpdateStatusDto
    {
        public int Id { get; set; }
        public Status Status { get; set; } // Assuming Status is a string, you can change it to an enum if needed
    }
}
