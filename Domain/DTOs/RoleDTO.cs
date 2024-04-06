using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Exceptions;

namespace Domain.DTOs
{
    public class RoleDTO
    {
        public long? Id { get; set; }
        public string? Name { get; set; }

        public int Gcode { get; set; }

        public string? Description { get; set; }

        public static implicit operator RoleDTO(Role role)  // convert Role to RoleDTO
        {
            return new RoleDTO
            {
                Id = role.Id,
                Name = role.Name,
                Gcode = role.Gcode,
                Description = role.Description
            };
        }

        public static implicit operator Role(RoleDTO role)  // convert RoleDTO to role
        {
            return new Role()
            {
                Id = role.Id ?? 0,
                Name = role.Name ?? string.Empty,
                Gcode = role.Gcode,
                Description = role.Description
            };
        }
    }
}
