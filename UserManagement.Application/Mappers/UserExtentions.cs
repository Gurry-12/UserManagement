using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Dtos;
using UserManagement.Domain.Entities;
using UserManagement.Shared.Enums;

namespace UserManagement.Application.Mappers
{
    public static class UserExtensions
    {
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Date = user.Date,
                Role = Enum.GetValues(typeof(Roles))
                            .Cast<Roles>()
                            .Where(r => r != Roles.None && user.Role.HasFlag(r))
                            .ToList()
            };
        }
    }
}
