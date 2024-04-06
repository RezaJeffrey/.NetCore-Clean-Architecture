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
        private readonly ICoreService<Role> CoreService;
        private readonly AuthUtilService AuthUtilService;

        public RoleAuthorizationHandler(ICoreService<Role> coreService, AuthUtilService authUtilService)
        {
            CoreService = coreService;
            AuthUtilService = authUtilService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleAuthorizationRequirement requirement)
        {
            if (requirement.IsMulti)
            {
                if (UserHasRoleOrParents(requirement.RequiredRoles))
                {
                    context.Succeed(requirement);
                }
            }
            else
            {
                if (UserHasRoleOrParents(requirement.RequiredRole ?? throw new Exception("RequiredRole must not be null")))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }

        private bool UserHasRoleOrParents(string requiredRole)
        {
            var userId = AuthUtilService.getUserId();

            var user_roles = AuthUtilService.getRoleClaims();

            var required_role = CoreService.Table()
                .Include(r => r.RoleParentRoles)
                .ThenInclude(p => p.Parent)
                .FirstOrDefault(r => r.Gcode == int.Parse(requiredRole));

            #region check null values
            if (required_role == null) throw new ServiceException("policy not correct!");
            if (user_roles == null) throw new ServiceException("user not found!");
            #endregion

            return user_roles.Any(r =>
                    r.Value == required_role.Gcode.ToString()
                    || required_role.RoleParentRoles.Any(p => p.Parent?.Gcode.ToString() == r.Value)
                );
        }

        private  bool UserHasRoleOrParents(List<string> requiredRoles)
        {
            var userId = AuthUtilService.getUserId()
                ?? throw new ServiceException("خطا در احراز هویت");

            var UserRole = AuthUtilService.GetUserRole()
                ?? throw new ServiceException("خطا در احراز هویت");

            List<Role> required_roles = CoreService.Table()
                .Include(r => r.RoleParentRoles)
                .ThenInclude(p => p.Parent)
                .Where(role => requiredRoles.Any(rr => rr == role.Gcode.ToString()))
                .ToList();

            #region check null values
            if (!required_roles.Any())
                throw new ServiceException("policy not correct!");
            #endregion

            bool result = required_roles
                .Any(role => 
                    role.Gcode.ToString() == UserRole ||
                    role.RoleParentRoles.Any(rp => rp.Parent?.Gcode.ToString() == UserRole)
                );

            return result;
        }
    }

}
