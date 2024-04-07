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

            if (UserIsInRole(requirement.RequiredRole ?? throw new Exception("RequiredRole must not be null")))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        private bool UserIsInRole(string requiredRole)
        {
            var userId = AuthUtilService.GetUserId();

            var user_role = AuthUtilService.GetUserRole();

            var required_role = CoreService.Table()
                .FirstOrDefault(r => r.Gcode == int.Parse(requiredRole));

            #region check null values
            if (required_role == null) throw new ServiceException("policy input not correct");
            if (user_role == null) throw new ServiceException("Failed fetching user role");
            #endregion

            return user_role == requiredRole;
        }
    }

}
