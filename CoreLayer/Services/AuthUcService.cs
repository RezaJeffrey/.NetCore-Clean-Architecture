using CoreLayer.Interfaces;
using Domain.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Utils.Exceptions;
using Utils.Services;

namespace CoreLayer.Services
{
    public class AuthUcService
    {
        private readonly ICoreService<User> CoreService;
        private readonly AuthUtilService AuthUtilService;
        private readonly IConfiguration _configuration;

        public AuthUcService(IConfiguration configuration, ICoreService<User> coreService, AuthUtilService authUtilService) 
        {
            CoreService = coreService;
            AuthUtilService = authUtilService;
            _configuration = configuration;
        }

        public async Task<bool> ValidateUserToken(User user)
        { 
            var userClaims = AuthUtilService.getClaims();
            var userId = AuthUtilService.getUserId();
            if (userId == null) throw new ServiceException("Token Not Valid, missing claim: UserId");

            User? User = await CoreService.FindByIdAsync((long)userId);
            if (User == null) throw new ServiceException("user doesn't exist");

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

            if (user == null) throw new ServiceException("User doesn't exist");

            var roles = user.UserRoles
                .Select(ur => ur.Role)
                .ToList();


            return roles;
        }

        public string CreateToken(User user, List<Role> roles, bool IsSuperAdmin = false)
        { 
            List<Claim> claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString(), "Identity"),
                new Claim("UserName", user.UserName, "Identity"),
            };

            var mainRole = user.UserRoles
                .FirstOrDefault(user => user.IsMainRole)?
                    .Role.Gcode.ToString() ?? string.Empty;


            claims.Add(new Claim("MainRole", mainRole, "Role"));

            if (!roles.Any())
                throw new ServiceException("User has no roles, account might not be accepted. Please Wait.");

            foreach (var role in roles)
            {
                claims.Add(
                        new Claim("Role", role.Gcode.ToString(), "Role")
                    );
            }


            return AuthUtilService.GenerateToken(claims, IsSuperAdmin);

        }
    }
}
