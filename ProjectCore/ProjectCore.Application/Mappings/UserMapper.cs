using ProjectCore.Application.Dtos.Users;
using ProjectCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Application.Mappings
{
    public static class UserMapper
    {
        public static UserDto ToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName.Value,
                Email = user.Email.Value,
                FullName = user.FullName?.Value,
                PhoneNumber = user.PhoneNumber?.Value,
                Gender = user.Gender?.ToString(),
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                AvatarUrl = user.Avatar?.Value,
                RoleIds = user.UserRoles.Select(r => r.RoleId).ToList(),
                Status = user.Status.ToString(),
                CreatedBy = user.CreatedBy,
                CreatedDate = user.CreatedDate,
                UpdatedBy = user.UpdatedBy,
                UpdatedDate = user.UpdatedDate
            };
        }
    }

}
