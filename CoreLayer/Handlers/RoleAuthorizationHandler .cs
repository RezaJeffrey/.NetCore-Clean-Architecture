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

namespace CoreLayer.Handlers
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


            var userdb = CoreService.Table<User>()
                             .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                             .FirstOrDefault(u => u.Id == 1);
            //                  .FirstOrDefault(u => u.Id == utils.AuthService.GetUserId() )

            var required_role = CoreService.Table()
                .Include(rr => rr.RoleParentPidNavigations).ThenInclude(p => p.Role)
                .FirstOrDefault(r => r.Gcode == int.Parse(requiredRole));  // check DDates

            #region check null values
            if (required_role == null)  throw new AppRuleException("policy not correct!");
            if (userdb == null) throw new AppRuleException("user not found!");
            #endregion

            return userdb.UserRoles.Any(ur =>
                    ur.Role.Gcode == required_role.Gcode
                 || required_role.RoleParentPidNavigations.Any(p => p.Role?.Gcode == ur.Role.Gcode)
                );
        }
    }

}
