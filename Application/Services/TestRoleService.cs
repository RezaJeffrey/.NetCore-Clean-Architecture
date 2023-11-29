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
        public CoreService<Role, RoleDTO> CoreService { get; set; }
        public TestRoleService(CoreService<Role, RoleDTO> coreService)
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
    }
}
