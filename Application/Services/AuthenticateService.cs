using CoreLayer.Interfaces;
using CoreLayer.Services;
using Domain.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utils.Exceptions;

namespace Application.Services
{
    public class AuthenticateService
    {
        private readonly IConfiguration _configuration;
        private readonly ICoreService<User, UserDTO> CoreService;
        private readonly TestRoleService _testRoleService;
        #region constructor
        public AuthenticateService(IConfiguration configuration, ICoreService<User, UserDTO> coreService, TestRoleService testRoleService)
        {
            _configuration = configuration;
            CoreService = coreService;
            _testRoleService = testRoleService;
        }
        #endregion
        public Task<string> CreateToken(User user, List<Role> roles)
        {
            throw new NotImplementedException();
        }
        public async Task<string> HashPassword(string password, string salt = "")
        {
            var passKey = _configuration.GetSection("AppSettings:PasswordHashKey");
            var saltAndKey = string.Empty;

            saltAndKey = (salt == string.Empty)
                ? saltAndKey = Convert.ToBase64String(GenerateSalt()) + passKey
                : salt + passKey;

            var hash = await Task.Run(() =>
            {
                return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.UTF8.GetBytes(saltAndKey),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
                );
            });

            return Convert.ToBase64String(hash);

        }
        public byte[] GenerateSalt()
        {
            var salt = new byte[128 / 8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            return salt;
        }
        public Task<bool> ValidateUserToken(User user, string token)
        {
            // check user exists
            // get roles
            // check with token roles, if one differs, return false
            throw new NotImplementedException();
        }
        public async Task<List<Role>> FetchUserRoles(long UserId)
        {
            await _testRoleService.GetRoleTest(UserId);
            var user = await CoreService.Table()
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == UserId);

            if (user == null) throw new AppRuleException("User doesn't exist");

            var roles = user.UserRoles
                .Select(ur => ur.Role)
                .Where(r => r.DeleteDate == null || r.DeleteDate == 0)
                .ToList();


            return roles;
        }
    }
}
