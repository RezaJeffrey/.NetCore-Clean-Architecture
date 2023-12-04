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
    public class TestRoleService
    {
        public ICoreService<Role, RoleDTO> CoreService { get; set; }
        public TestRoleService(ICoreService<Role, RoleDTO> coreService)
        {
            CoreService = coreService;
        }

        public async Task<List<RoleDTO>> GetRolesTest()
        {
            var role = await CoreService.Table().ToListAsync();
            return ObjectMapper.MapList<Role, RoleDTO>(role);
        }

        public async Task<RoleDTO> GetRoleTest(long Id)
        {
            var role = await CoreService.FindByIdAsync(Id);
            if (role == null) throw new AppRuleException("no such Item in database or you don't have sufficient permissions");

            return ObjectMapper.MapObject<Role, RoleDTO>(role);
        }

        public async Task<List<UserRole>> GetUserRolesTest()
        {
            var brands = await CoreService.Table<UserRole>().ToListAsync();
            return brands;
        }

        public async Task CreateRole(RoleDTO input)
        {
            var role = ObjectMapper.MapObject<RoleDTO, Role>(input);


            await CoreService.Create(role, false);
            await CoreService.CommitAsync();
        }

        public async Task DeleteRoleByDTO(RoleDTO input)
        {

            var deleted = await CoreService.Table()
                .Where(i => i.Name == input.Name || i.Gcode == input.Gcode)
                .FirstOrDefaultAsync();

            if (deleted == null) throw new AppRuleException("Item does not exist in DataBase or you don't have sufficient permissions");

            await CoreService.Delete(deleted.Id, false);
            await CoreService.CommitAsync();

        }
        public bool CheckHandler(string requiredRole)
        {

            var userdb = CoreService.Table<User>()
                             .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                             .FirstOrDefault(u => u.Id == 1);
            
            var required_role = CoreService.Table()
                .Include(r =>
                    r.RoleParentRoles.Where(rp => rp.DeleteDate == null || rp.DeleteDate == 0)
                    )
                .ThenInclude(p => p.Parent)
                .FirstOrDefault(r => r.Gcode == int.Parse(requiredRole));

            #region check null values
            if (required_role == null) throw new AppRuleException("policy not correct!");
            if (userdb == null) throw new AppRuleException("user not found!");
            #endregion

            return userdb.UserRoles.Any(ur =>
                    ur.Role.Gcode == required_role.Gcode
                 || required_role.RoleParentRoles.Any(p => p.Role?.Gcode == ur.Role.Gcode)
                );

        }
        public void CheckQueryFilter()
        {
            var userdb = CoreService.Table<User>()
                             .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                             .FirstOrDefault(u => u.Id == 1);

            var required_role = CoreService.Table()
                .Include(r => r.RoleParentRoles)
                .ThenInclude(p => p.Parent)
                .FirstOrDefault(rr => rr.Id == 2);
            var all = CoreService.TableAll().ToList();
            var queried = CoreService.Table().ToList();
        }
    }
}
