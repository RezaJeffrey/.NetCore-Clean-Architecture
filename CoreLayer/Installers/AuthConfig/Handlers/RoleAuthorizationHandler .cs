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

namespace CoreLayer.Installers.AuthConfig.Handlers
{
    public class RoleAuthorizationHandler : AuthorizationHandler<RoleAuthorizationRequirement>
    {
        private readonly ICoreService<Role, RoleDTO> CoreService;

        public RoleAuthorizationHandler(ICoreService<Role, RoleDTO> coreService)
        {
            CoreService = coreService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleAuthorizationRequirement requirement)
        {
            if (UserHasRoleOrParents(context.User, requirement.RequiredRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        private bool UserHasRoleOrParents(ClaimsPrincipal user, string requiredRole)
        {
            var userId = long.Parse(user.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value); // TODO remove this after implementation of utils.authservice
            var userdb = CoreService.Table<User>()
                .Include(u =>
                    u.UserRoles.Where
                            (ur => ur.DeleteDate == null || ur.DeleteDate == 0)
                    ).ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.Id == userId); // TODO use Utils.AuthService.GetUserID from Claims

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
    }

}
