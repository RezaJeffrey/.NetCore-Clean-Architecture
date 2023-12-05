using CoreLayer.Interfaces;
using Domain.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Utils.Exceptions;
using Utils.Services;

namespace CoreLayer.Services
{
    public class AuthUcService
    {
        private readonly ICoreService<User, UserDTO> CoreService;
        private readonly AuthUtilService AuthUtilService;
        private readonly IConfiguration _configuration;

        public AuthUcService(IConfiguration configuration, ICoreService<User, UserDTO> coreService, AuthUtilService authUtilService) 
        {
            CoreService = coreService;
            AuthUtilService = authUtilService;
            _configuration = configuration;
        }

        public async Task<bool> ValidateUserToken(User user)
        { // TODO : modify Installers
            var userClaims = AuthUtilService.getClaims();
            var userId = AuthUtilService.getUserId();
            if (userId == null) throw new AppRuleException("Token Not Valid, missing claim: UserId");

            User? User = await CoreService.FindByIdAsync((long)userId);
            if (User == null) throw new AppRuleException("user doesn't exist");

            var UserRoles  = CoreService.Table<UserRole>()
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.Role.Gcode)
                .ToList();

            var TokenRoleGcodes = userClaims
                .Where(claim => claim.Type == "Role")
                .Select(c => int.Parse(c.Value))
                .ToList();

            var tokenRolesNotInDBRoles = UserRoles.Except(TokenRoleGcodes);
            var DBRolesNotInToken = TokenRoleGcodes.Except(UserRoles);

            return !tokenRolesNotInDBRoles.Any() && !DBRolesNotInToken.Any();
        }
        public async Task<List<Role>> BindUserRoles(long UserId)
        {
            var user = await CoreService.Table()
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == UserId);

            if (user == null) throw new AppRuleException("User doesn't exist");

            var roles = user.UserRoles
                .Select(ur => ur.Role)
                .ToList();


            return roles;
        }

    }
}
