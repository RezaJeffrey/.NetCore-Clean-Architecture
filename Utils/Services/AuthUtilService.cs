using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utils.Exceptions;


namespace Utils.Services
{
    public class AuthUtilService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor HttpContextAccessor;
        public AuthUtilService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            HttpContextAccessor = httpContextAccessor;
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

        public string GenerateToken(List<Claim> claims)
        {
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

        public async Task<bool> ValidatePassword(string password, byte[] salt, string hash)
        {
            var passwordHash = await HashPassword(password, salt);
            return hash == passwordHash.hash;
        }

        public IEnumerable<Claim> getClaims()
        {
            var securityTokenHandler = new JwtSecurityTokenHandler();
            var accessToken = GetUserToken();
            if(securityTokenHandler.CanReadToken(accessToken))
            {
                var descriptedToken = securityTokenHandler.ReadJwtToken(accessToken);

                return descriptedToken.Claims;
            }
            return new List<Claim>();
        }

        public string? getUserName()
        {
            return getClaims().Where(claim => claim.Type == "UserName").FirstOrDefault()?.Value;
        }
        public long? getUserId()
        {
            string? UserId = getClaims().Where(claim => claim.Type == "UserId").FirstOrDefault()?.Value;
            return long.TryParse(UserId, out long userId) ? userId : null;   
        }
        public List<Claim> getRoleClaims()
        {
            return getClaims().Where(claim => claim.Type == "Role").ToList();
        }
        public string? GetUserToken()
        {
            string? accessToken = HttpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Authorization];
            accessToken = accessToken?.Replace("Bearer ", "");
            return accessToken;
        }
        public string? GetUserRole()
        {
            return getClaims().Where(claim => claim.Type == "MainRole").FirstOrDefault()?.Value;
        }
    }
}
