using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Extentions;

namespace Domain.DTOs
{
    public class UserDTO
    {
        public long? Id { get; set; }

        public string? UserName { get; set; }

        public string? Name { get; set; }

        public string? CreateDate { get; set; }

        public RoleDTO? Role { get; set; }

        public static implicit operator UserDTO(User user)
        {
            return new UserDTO()
            {
                Id = user.Id,   
                UserName = user.UserName,
                Name = user.Name,
                Role = user.Role,
                CreateDate = user.CreateDate.ToShamsiShortDate() ?? "N/A",
            };
        }
    }
}
