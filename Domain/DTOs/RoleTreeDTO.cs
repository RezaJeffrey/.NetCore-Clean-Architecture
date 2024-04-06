using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Utils.Exceptions;

namespace Domain.DTOs
{
    public class RoleTreeDTO
    {
        public long? Id {  get; set; }
        public string? title { get; set; }
        public int? Gcode { get; set; }

        public List<RoleTreeDTO>? children { get; set; }

        public static implicit operator RoleDTO(RoleTreeDTO dto)
        {
            return new RoleDTO()
            {
                Id = dto.Id,
                Name = dto.title,
                Gcode = dto.Gcode ?? throw new ServiceException("Gcode can't be null")
            };
        }
    }
}
