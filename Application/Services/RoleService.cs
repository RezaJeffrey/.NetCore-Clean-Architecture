using CoreLayer.Interfaces;
using CoreLayer.Services;
using Domain.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Exceptions;
using Utils.Mappings;

namespace Application.Services
{
    public class RoleService
    {
        public ICoreService<Role, RoleDTO> CoreService { get; set; }
        public RoleService(ICoreService<Role, RoleDTO> coreService)
        {
            CoreService = coreService;
        }

        public async Task<bool> CheckRoleAvailable(List<RoleDTO> roles)
        {
            var db_roles = await CoreService.Table().Select(role => role.Gcode).ToListAsync();
            foreach (int? role in roles.Select(r => r.Gcode))
            {
                if (role != null && !db_roles.Contains((int)role) )
                    throw new BusinessException("role doesn't exist in database");
            }
            return true;
        }
    }
}
