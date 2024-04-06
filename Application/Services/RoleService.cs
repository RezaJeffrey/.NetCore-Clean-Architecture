using CoreLayer.Extensions;
using CoreLayer.Interfaces;
using CoreLayer.Services;
using Domain.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Exceptions;
using Utils.Mappings;
using Utils.Services;

namespace Application.Services
{
    public class RoleService
    {
        public ICoreService<Role> CoreService { get; set; }
        private readonly AuthUtilService AuthUtilService;
        public RoleService(ICoreService<Role> coreService, AuthUtilService authUtilService)
        {
            CoreService = coreService;
            AuthUtilService = authUtilService;
        }

        public async Task<RoleDTO> CreateRole(RoleDTO dto, long? parentID = null)  
        {
            Role role = dto;

            var Exists = await CoreService.Table()
                .AnyAsync(r => r.Id == dto.Id || r.Gcode == dto.Gcode);

            if (Exists)
                throw new ServiceException("نقشی با این کد وجود دارد");

            await CoreService.Create(role, false);

            if (parentID != null && parentID != 0)  // create parent relation
            {
                var RoleParent = new RoleParent();
                RoleParent.Role = role;
                RoleParent.ParentId = (long)parentID;

                RoleParent.CreateDate = DateTime.Now.Ticks;
                RoleParent.CreateUserId = AuthUtilService.getUserId();

                await CoreService.GetDb<RoleParent>()
                    .AddAsync(RoleParent);
            }

            await CoreService.CommitAsync();
            return role;    
        }

        public async Task<List<RoleDTO>> BindRoles()
        {
            var UserRole = AuthUtilService.GetUserRole();

            if (UserRole != null)
            {
                if(UserRole.IsAdminOrSuperAdmin())  // Define this extension as your project roles 
                {
                    // Admin can see the list of all roles
                    return 
                        await CoreService.Table()
                            .Select(role => (RoleDTO)role)
                            .ToListAsync();
                }
            }

            return
                await CoreService.Table()
                    .Where(role => role.Gcode != 3 && role.Gcode != 1)  // Define this extension as your project roles
                    .Select(role => (RoleDTO)role)
                    .ToListAsync();
        }

        //public async Task<bool> CheckParentIsValid(int parentCode, int childCode)
        //{
        //    var Roles =
        //        await CoreService.Table()
        //        .Include(r => r.RoleParentRoles)
        //        .Where(r => r.Gcode == parentCode || r.Gcode == childCode)
        //        .ToListAsync();

        //    var parentRole = 
        //        Roles.FirstOrDefault(r => r.Gcode == parentCode)
        //            ?? throw new ServiceException("فیلد parentCode معتبر نمی باشد");

        //    var childRole = 
        //        Roles.FirstOrDefault(r => r.Gcode == childCode)
        //            ?? throw new ServiceException("فیلد childCode معتبر نمی باشد");


        //    if (
        //        !childRole.RoleParentRoles.Any(rp => rp.ParentId == parentRole.Id)
        //    )
        //        return false;

        //    return true;
        //}
    }
}