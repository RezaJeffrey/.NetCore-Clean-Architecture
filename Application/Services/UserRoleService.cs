using CoreLayer.Interfaces;
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

namespace Application.Services
{
    public class UserRoleService
    {

        public ICoreService<UserRole, UserRoleDTO> CoreService { get; set; }
        public TestRoleService TestRoleService { get; set; }
        public UserRoleService(ICoreService<UserRole, UserRoleDTO> coreService, TestRoleService testRoleService)
        {
            CoreService = coreService;
            TestRoleService = testRoleService;
        }

        public async Task<Role> AddUserRole(long userId, int roleGcode, bool save = true)
        {
            User? user = CoreService.Table<User>().Where(user => user.Id == userId).FirstOrDefault();
            if (user == null) { throw new BusinessException("User not found"); }

            Role? role = await CoreService.Table<Role>()
                .FirstOrDefaultAsync(role => role.Gcode == roleGcode);
            if(role == null) { throw new BusinessException("Role not valid"); }

            UserRole userRole = new UserRole();
            userRole.UserId = userId;
            userRole.RoleId = role.Id;

            await CoreService.Create(userRole, save);

            return role;
        }

        public async Task<List<Role>> AddUserRole(long userId, List<int> roleGcodes, bool save = true)
        {
            User? user = CoreService.Table<User>().Where(user => user.Id == userId).FirstOrDefault();
            if (user == null) { throw new BusinessException("User not found"); }

            var roles = await CoreService.Table<Role>()
                .Where(role => roleGcodes.Contains(role.Gcode))
                .ToListAsync();

            if (!roles.Any()) throw new BusinessException("role not valid");

            foreach(var role in roles)
            {
                UserRole userRole = new UserRole();
                userRole.UserId = userId;
                userRole.RoleId = role.Id;

                await CoreService.Create(userRole, false);
            }

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
