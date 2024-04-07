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

        public string CreateToken(User user, bool IsSuperAdmin = false)
        { 
            List<Claim> claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString(), "Identity"),
                new Claim("UserName", user.UserName, "Identity"),
            };

            var userRole = user.Role?.Gcode.ToString() ?? "N/A";


            claims.Add(new Claim("Role", userRole, "Role"));


            return AuthUtilService.GenerateToken(claims, IsSuperAdmin);

        }
    }
}
