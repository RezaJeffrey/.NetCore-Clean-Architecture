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

        public async Task<bool> CheckRoleAvailable(List<RoleDTO> roles)
        {
            var db_roles = await CoreService.Table().Select(role => role.Gcode).ToListAsync();
            foreach (int? role in roles.Select(r => r.Gcode))
            {
                if (role != null && !db_roles.Contains((int)role) )
                    throw new ServiceException("نقش مورد نظر یافت نشد");
            }
            return true;
        }

        public async Task<RoleDTO> CreateRole(RoleDTO dto, long? parentID = null)  // TODO Test
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

        public async Task CreateRoleFromTree(string Json)
        {
            await CoreService.BeginTransaction();

            var UserId = AuthUtilService.getUserId();
            var Now = DateTime.Now.Ticks;

            var Roles = await CoreService.Table()
                .Include(r => r.RoleParentParents)
                .ThenInclude(rp => rp.Role)
                .ToListAsync();


            dynamic raw_roles = JArray.Parse(Json);

            foreach(var role in raw_roles)
            {
                RoleTreeDTO Role = JsonConvert.DeserializeObject<RoleTreeDTO>(role);

                if (Roles.Any(r => r.Gcode == Role.Gcode || r.Id == Role.Id))  // Role Exist in database  // TODO change to one condition if body empty
                {
                        
                }
                else
                {
                    await CreateRole(Role);
                }

                var ParentID = Role.Id;

                while(true)  // continue checking childs
                {
                    if (!((JObject)role).TryGetValue("children", out var child))  // end of depth
                    {
                        break;
                    }

                    foreach(var child_role in child)
                    {
                        RoleTreeDTO ChildRole = JsonConvert.DeserializeObject<RoleTreeDTO>(child_role.ToString())
                            ?? throw new ServiceException("deserialization for ChildRole Failed");

                        // check Exists
                        var dbRole = Roles.FirstOrDefault(r => r.Gcode == Role.Gcode);
                        if (dbRole != null)  // yes
                        {
                            // check if parent changed
                            bool ParentAdded = dbRole.RoleParentParents.Any(p => p.Role.Gcode == dbRole.Gcode);
                            bool ParentDeleted = !dbRole.RoleParentParents.Any(p => p.Role.Gcode == Role.Gcode);

                            if (ParentAdded)
                            {
                                RoleParent parent = new RoleParent()
                                {
                                    RoleId = dbRole.Id,
                                    ParentId = ParentID ?? throw new ServiceException("Parent ID not found"),
                                    CreateDate = Now,
                                    CreateUserId = dbRole.Id
                                };

                                await CoreService.GetDb<RoleParent>()
                                    .AddAsync(parent);
                            }
                            if (ParentDeleted)
                            {
                                var parent = await CoreService.Table<RoleParent>()
                                    .FirstOrDefaultAsync(rp => rp.RoleId == dbRole.Id && rp.ParentId == ParentID);
                                if (parent != null)
                                {
                                    parent.DeleteDate = Now;
                                    parent.DeleteUserId = UserId;

                                    await CoreService.GetDb<RoleParent>()
                                        .AddAsync(parent);
                                }

                            }
                        }
                        else
                        {

                            // else
                            // Create new Role with ParentID
                            var createdRole = await CreateRole(new RoleDTO()
                            {
                                Name = ChildRole.title ?? string.Empty,
                                Gcode = ChildRole.Gcode ?? throw new ServiceException("role Gcode can't be null")
                            }, parentID: ParentID);

                        }

                    }

                    ParentID = child.Value<long>("RequestId");
                    await CoreService.CommitAsync();
                }

            }
        }

        public async Task<List<RoleTreeDTO>> BindRolesTree()
        {

            var Roles = await CoreService.Table()
                .Include(r => r.RoleParentParents)
                .ThenInclude(rp => rp.Role)
                .Include(r => r.RoleParentRoles)
                .Where(r => !r.RoleParentRoles.Any())
                .ToListAsync();
            
            var tree = Roles.Select(BuildTree).ToList();

            return tree;
        }

        private RoleTreeDTO BuildTree(Role role)
        {
            var treeObj = new RoleTreeDTO
            {
                Id = role.Id,
                title = role.Name ?? string.Empty,
                Gcode = role.Gcode
            };

            var children = role.RoleParentParents
                .Select(rp =>
                {
                    rp.Role.RoleParentParents = CoreService.Table()
                    .Include(r => r.RoleParentParents)
                    .ThenInclude(rp => rp.Role)
                    .FirstOrDefault(r => r.Id == rp.Role.Id)?
                    .RoleParentParents ?? new List<RoleParent>();

                    return BuildTree(rp.Role);
                }).ToList();

            if (children.Any())
            {
                treeObj.children = children;
            }

            return treeObj;
        }

        /// <summary>
        ///     returns requested roe with the list of its parents
        /// </summary>
        /// <param name="gCode"></param>
        public async Task<Role> GetRoleByGcode(int gCode)
        {
            return
                await CoreService
                    .Table()
                    .Include(r => r.RoleParentParents)  // Todo: check if it's being bound properly
                        .ThenInclude (rp => rp.Role)
                    .Include(r => r.RoleParentRoles)
                        .ThenInclude(rp => rp.Parent)
                    .FirstOrDefaultAsync(r => r.Gcode == gCode)
                        ?? throw new ServiceException("Invalid Role");
        }

        public async Task<List<RoleDTO>> BindRoles()
        {
            var UserRole = AuthUtilService.GetUserRole();

            if (UserRole != null)
            {
                if(UserRole.IsAdminOrSuperAdmin())
                {
                    return 
                        await CoreService.Table()
                            .Select(role => (RoleDTO)role)
                            .ToListAsync();
                }
            }

            return
                await CoreService.Table()
                    .Where(role => role.Gcode != 3 && role.Gcode != 1)
                    .Select(role => (RoleDTO)role)
                    .ToListAsync();
        }

        public async Task<bool> CheckParentIsValid(int parentCode, int childCode)
        {
            var Roles =
                await CoreService.Table()
                .Include(r => r.RoleParentRoles)
                .Where(r => r.Gcode == parentCode || r.Gcode == childCode)
                .ToListAsync();

            var parentRole = 
                Roles.FirstOrDefault(r => r.Gcode == parentCode)
                    ?? throw new ServiceException("فیلد parentCode معتبر نمی باشد");

            var childRole = 
                Roles.FirstOrDefault(r => r.Gcode == childCode)
                    ?? throw new ServiceException("فیلد childCode معتبر نمی باشد");


            if (
                !childRole.RoleParentRoles.Any(rp => rp.ParentId == parentRole.Id)
            )
                return false;

            return true;
        }
    }
}