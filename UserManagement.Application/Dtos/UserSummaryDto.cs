using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.Dtos
{
    public class UserSummaryDto
    {
        public int ClinicianCount { get; set; }
        public int StaffCount { get; set; }
        public int DeactivatedClinicianCount { get; set; }
    }
}
