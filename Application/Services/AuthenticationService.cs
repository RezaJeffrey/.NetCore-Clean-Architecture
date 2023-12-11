using CoreLayer.Interfaces;
using CoreLayer.Services;
using Domain.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Exceptions;
using Utils.Extentions;
using Utils.Mappings;
using Utils.Services;

namespace Application.Services
{
    public class AuthenticationService
    {
        private readonly ICoreService<User, UserDTO> CoreService;
        private readonly AuthUcService AuthUcService;
        private readonly AuthUtilService AuthUtilService;
        private readonly TestRoleService RoleService;
        private readonly LoginLogService LoginLogService;
        private readonly UserRoleService UserRoleService;
        public AuthenticationService(
            AuthUcService authUcService,
            ICoreService<User, UserDTO> coreService,
            AuthUtilService authUtilService,
            TestRoleService roleService,
            LoginLogService loginLogService,
            UserRoleService userRoleService
            )
        {
            CoreService = coreService;
            AuthUcService = authUcService;
            AuthUtilService = authUtilService;
            RoleService = roleService;
            LoginLogService = loginLogService;
            UserRoleService = userRoleService;
        }
        
        public async Task<string> RefreshToken(string accessToken)
        {
            var deserialized_token = AuthUtilService.getClaims(accessToken);

            long userId = long.Parse(deserialized_token.GetClaim("UserId").Value);
            User? user = await CoreService.FindByIdAsync(userId);

            if (user == null)
                throw new AppRuleException("User Not Found!");

            var userRoles = CoreService.Table<UserRole>()
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role)
                .ToList();

            return AuthUcService.CreateToken(user, userRoles);
        }
        
        public async Task<string> GetAccessToken(AuthDTO user_dto)
        {
            User? user = CoreService.Table()
                .Include(u => u.MainRole)
                    .ThenInclude(mr => mr.Role)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.UserName == user_dto.UserName);

            if (user == null)
                throw new AppRuleException("Wrong username or password");


            // TODO log login attempts
            var Now = DateTime.Now.Ticks;
            var logs = CoreService.Table<LogLogin>()
                .Where(l => l.UserId == user.Id && l.ExpDate > Now && !l.IsSuccess);

            if (logs.Count() > 5) 
                throw new AppRuleException(
                        "Account has been limited due to 5 failed login attempts. Please try again later or inform us."
                    );

            LogLogin log = new LogLogin
            {
                UserId = user.Id,
                IpAddress = AuthUtilService.GetClientIp(),
                ExpDate = DateTime.Now.AddDays(1).Ticks
            };


            bool passwordMatch = await AuthUtilService
                .ValidatePassword(user_dto.Password, user.PasswordSalt, user.PasswordHash);
            
            if (!passwordMatch)
            {
                // Insert log
                log.IsSuccess = false;
                await LoginLogService.CoreService.Create(log);

                throw new AppRuleException("Wrong username or password");
            }
                

            var userRoles = user.UserRoles.Select(ur => ur.Role).ToList();
            var token = AuthUcService.CreateToken(user, userRoles);

            log.IsSuccess = true;
            await LoginLogService.CoreService.Create(log);

            return token;
        }
        public async Task<string> Register(AuthDTO user_dto)
        {
            try
            {
                await CoreService.BeginTransaction();

                if (user_dto.Password != user_dto.PasswordRepeat)
                    throw new AppRuleException("Passwords must match");

                bool userExists = CoreService.Table()
                    .FirstOrDefault(user => user.UserName == user_dto.UserName) != null;

                if (userExists)
                    throw new AppRuleException(
                            "a user with this username already exists, Consider Login or enter new username"
                        );


                var RegisterUser = ObjectMapper.MapObject<AuthDTO, User>(user_dto);

                if (user_dto.MainRoleGcode.IsNullOrEmpty()) user_dto.MainRoleGcode = "10";

                var main_role = CoreService.Table<Role>()
                    .FirstOrDefault(role => role.Gcode.ToString() == user_dto.MainRoleGcode);
                
                if (main_role == null)
                    throw new AppRuleException("Role is not valid");

                RegisterUser.MainRoleId = main_role.Id;

                var hashService = await AuthUtilService.HashPassword(user_dto.Password);
                RegisterUser.PasswordHash = hashService.hash;
                RegisterUser.PasswordSalt = hashService.salt;

                await CoreService.Create(RegisterUser, true);

                var createdUser = await CoreService.Table()
                    .Include(user => user.MainRole)
                    .FirstOrDefaultAsync(
                            user => user.UserName.ToLower() == RegisterUser.UserName.ToLower()
                        );
                
                if (createdUser == null) throw new AppRuleException("User Creation Failed"); // TODO test failure

                List<Role> roles = new List<Role>();

                if (user_dto.rolesToRegister.Any())
                {
                    var roleAvailable = await RoleService.CheckRoleAvailable(user_dto.rolesToRegister.ToList());
                    
                    if (!roleAvailable)
                        throw new AppRuleException("Input role not valid");

                    var roles_to_add = user_dto.rolesToRegister
                        .Where(r => r != null)
                        .Select(r => r.Gcode)
                        .ToList();

                    roles = await UserRoleService.AddUserRole(createdUser.Id, roles_to_add);
                }
                else
                {
                    int guestRole = 10; 
                    roles.Add(await UserRoleService.AddUserRole(createdUser.Id, guestRole));
                }

                var exists = roles.Where(r => r.Gcode == createdUser.MainRole?.Role.Gcode).Any();
                
                if (!exists)
                    throw new AppRuleException("User MainRole doesn't match selected roles to add");

                await CoreService.CommitAsync();
                await CoreService.CommitTransaction();

                return AuthUcService.CreateToken(createdUser, roles);
            }
            catch(Exception)
            {
                await CoreService.RollBackTransaction();
                throw;
            }
        }

    }
}
