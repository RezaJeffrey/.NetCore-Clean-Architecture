using CoreLayer.Interfaces;
using CoreLayer.Services;
using Domain.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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
        // TODO move methods to Auth Utils
        public string CreateToken(User user, List<Role> roles)
        { // TODO add user main role in DB and set it as a claim
            List<Claim> claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString(), "Identity"),
                new Claim("UserName", user.UserName, "Identity"),
                //new Claim("UserRole", user.RoleGcode, "MainRole"),
            };

            foreach (var role in roles)
            {
                claims.Add(
                        new Claim("Role", role.Gcode.ToString(), "Role")
                    );
            }


            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                            _configuration.GetSection("AppSettings:TokenKey").Value ?? string.Empty
                        )
                );

            SigningCredentials signingCredentials = new SigningCredentials(
                    tokenKey,
                    SecurityAlgorithms.HmacSha512Signature
                );

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = signingCredentials,
                Expires = DateTime.Now.AddDays(1)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityToken token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);

        }
        public async Task<(string hash, byte[] salt)> HashPassword(string password, byte[]? salt = null)
        {
            var passKey = _configuration.GetSection("AppSettings:PasswordHashKey");
            var generatedSalt = (salt == null) ? GenerateSalt() : salt;
            var saltAndKey = Convert.ToBase64String(generatedSalt) + passKey;


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

            return (Convert.ToBase64String(hash), generatedSalt);

        }
        public byte[] GenerateSalt()
        {
            var salt = new byte[128 / 8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            return salt;
        }

        public async Task<bool> ValidatePassword(string password, byte[] salt, string hash)
        {
            var passwordHash = await HashPassword(password, salt);
            return hash == passwordHash.hash;
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

        public async Task<User?> GetUserById(long UserId)
        {
            return await CoreService.FindByIdAsync(UserId);
        }
       
        // Login Method
        // Register Method
    }
}
