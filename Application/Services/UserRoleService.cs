﻿using CoreLayer.Interfaces;
using Domain.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Utils.Exceptions;

namespace Application.Services
{
    public class UserRoleService
    {

        public ICoreService<UserRole, UserRoleDTO> CoreService { get; set; }
        public UserRoleService(ICoreService<UserRole, UserRoleDTO> coreService)
        {
            CoreService = coreService;
        }

        public async Task<Role> AddUserRole(long userId, int roleGcode, bool isMain = false, bool save = true)
        {
            User? user = CoreService.Table<User>().Where(user => user.Id == userId).FirstOrDefault();
            if (user == null) { throw new BusinessException("User not found"); }

            Role? role = await CoreService.Table<Role>()
                .FirstOrDefaultAsync(role => role.Gcode == roleGcode);
            if(role == null) { throw new BusinessException("Role not valid"); }

            UserRole userRole = new UserRole();
            userRole.UserId = userId;
            userRole.RoleId = role.Id;
            userRole.IsMainRole = isMain;

            await CoreService.Create(userRole, save);

            return role;
        }

        public async Task<List<Role>> AddUserRole(long userId, List<int> roleGcodes, int mainRoleCode = 0, bool save = true)
        {
            User? user = CoreService.Table<User>()
                .Where(user => user.Id == userId)
                .FirstOrDefault();

            if (user == null) 
                throw new BusinessException("User not found"); 

            var roles = await CoreService.Table<Role>()
                .Where(role => roleGcodes.Contains(role.Gcode))
                .ToListAsync();

            if (!roles.Any())
                throw new BusinessException("role not valid");

            bool mainExistsInput = false;
            foreach(var role in roles)
            {
                UserRole userRole = new UserRole();
                userRole.UserId = userId;
                userRole.RoleId = role.Id;

                if (role.Gcode == mainRoleCode)
                {
                    userRole.IsMainRole = true;
                    mainExistsInput = true;
                }
                await CoreService.Create(userRole, false);
            }

            if (!mainExistsInput && mainRoleCode != 0)
                throw new BusinessException("Main role does not exist in sent roles.");

            if (save) { await CoreService.CommitAsync(); }
            return roles;
        }
        
        public async Task<UserRole> GetUserRole(long userId, long roleId)
        {
            var userRole = await CoreService.Table()
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (userRole == null)
                throw new BusinessException("Operation Failure");

            return userRole;
        }


    }
}
