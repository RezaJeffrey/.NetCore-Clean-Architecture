using Azure.Identity;
using CoreLayer.Interfaces;
using Domain.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Utils.Exceptions;
using Utils.Extentions;
using Utils.Services;

namespace CoreLayer.Installers.AuthConfig.Handlers
{
    public class RoleAuthorizationHandler : AuthorizationHandler<RoleAuthorizationRequirement>
    {
        private readonly ICoreService<Role, RoleDTO> CoreService;
        private readonly AuthUtilService AuthUtilService;

        public RoleAuthorizationHandler(ICoreService<Role, RoleDTO> coreService, AuthUtilService authUtilService)
        {
            CoreService = coreService;
            AuthUtilService = authUtilService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleAuthorizationRequirement requirement)
        {
            if (UserHasRoleOrParents(requirement.RequiredRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        private bool UserHasRoleOrParents(string requiredRole)
        {
            var userId = AuthUtilService.getUserId();

            //var userdb = CoreService.Table<User>()
            //    .Include(u =>u.UserRoles)
            //    .ThenInclude(ur => ur.Role)
            //    .FirstOrDefault(u => u.Id == userId); 

            var user_roles = AuthUtilService.getRoleClaims();

            var required_role = CoreService.Table()
                .Include(r => r.RoleParentRoles)
                .ThenInclude(p => p.Parent)
                .FirstOrDefault(r => r.Gcode == int.Parse(requiredRole));

            #region check null values
            if (required_role == null) throw new BusinessException("policy not correct!");
            if (user_roles == null) throw new BusinessException("user not found!");
            #endregion

            return user_roles.Any(r =>
                    r.Value == required_role.Gcode.ToString()
                    || required_role.RoleParentRoles.Any(p => p.Role?.Gcode.ToString() == r.Value)
                );
            //return userdb.UserRoles.Any(ur =>
            //        ur.Role.Gcode == required_role.Gcode
            //     || required_role.RoleParentRoles.Any(p => p.Role?.Gcode == ur.Role.Gcode)
            //    );
        }
    }

}
