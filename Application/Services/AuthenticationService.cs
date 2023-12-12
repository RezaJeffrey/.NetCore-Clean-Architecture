using CoreLayer.Interfaces;
using CoreLayer.Services;
using Domain.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

            User? user = await CoreService.Table()
                .Include(user => user.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(user => user.Id == userId);
            
            if (user == null)
                throw new BusinessException("User Not Found!");

            var userRoles = user.UserRoles.Select(ur => ur.Role).ToList();

            return AuthUcService.CreateToken(user, userRoles);
        }
        
        public async Task<string> GetAccessToken(AuthDTO user_dto)
        {
            User? user = CoreService.Table()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.UserName == user_dto.UserName);

            if (user == null)
                throw new BusinessException("Wrong username or password");


            var Now = DateTime.Now.Ticks;
            var logs = CoreService.Table<LogLogin>()
                .Where(l => l.UserId == user.Id && l.ExpDate > Now && !l.IsSuccess);

            if (logs.Count() > 5) 
                throw new BusinessException(
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

                throw new BusinessException("Wrong username or password");
            }
                

            var userRoles = user.UserRoles.Select(ur => ur.Role).ToList();
            var token = AuthUcService.CreateToken(user, userRoles);

            log.IsSuccess = true;
            await LoginLogService.CoreService.Create(log);

            return token;
        }

        public async Task CreateUser(AuthDTO userDTO)
        {

            if (userDTO.Password.IsNullOrEmpty())
                throw new BusinessException("Invalid input.");

            if (userDTO.Password != userDTO.PasswordRepeat)
                throw new BusinessException("Passwords must match.");

            bool userExists = CoreService.Table()
                .FirstOrDefault(user => user.UserName == userDTO.UserName) != null;

            if (userExists)
                throw new BusinessException(
                        "a user with this username already exists, Consider Login or enter new username."
                    );

            var RegisterUser = ObjectMapper.MapObject<AuthDTO, User>(userDTO);

            var hashService = await AuthUtilService.HashPassword(userDTO.Password);
            RegisterUser.PasswordHash = hashService.hash;
            RegisterUser.PasswordSalt = hashService.salt;

            await CoreService.Create(RegisterUser);

        }

        public async Task<User?> GetUserByUsername(string username) // TODO add username index in DB
        {
            return await CoreService.Table()
                .FirstOrDefaultAsync(
                        user => user.UserName.ToLower() == username.ToLower()
                    );
        }

        public async Task UserSignUp(AuthDTO userDTO) 
        {
            try
            {
                await CreateUser(userDTO);
                var created_user = await GetUserByUsername(userDTO.UserName);

                if (created_user == null)
                    throw new BusinessException("User Creation Failed");

                #region Add Default Guest UserRole
                int guest_role = 10;

                var guestRole = CoreService.Table<Role>()
                    .FirstOrDefault(role => role.Gcode == guest_role);

                if (guestRole == null)
                    throw new BusinessException("Role is not valid");

                await UserRoleService.AddUserRole(created_user.Id, guestRole.Gcode, isMain: true, save: false);

                #endregion

                await CoreService.CommitAsync();
                await CoreService.CommitTransaction();
            }
            catch (Exception)
            {
                await CoreService.RollBackTransaction();
                throw;
            }
        }
        public async Task RegisterUser(AuthDTO userDTO) 
        {
            try
            {
                await CoreService.BeginTransaction();

                await CreateUser(userDTO);
                var created_user = await GetUserByUsername(userDTO.UserName);

                if (created_user == null)
                    throw new BusinessException("User Creation Failed");

                // create user Roles based on roles sent
                #region Add UserRoles

                int guestRole = 10;
                if (userDTO.MainRoleGcode.IsNullOrEmpty())
                    userDTO.MainRoleGcode = guestRole.ToString();

                List<Role> roles = new List<Role>();
                if (userDTO.rolesToRegister.Any())
                {
                    var roleAvailable = await RoleService.CheckRoleAvailable(userDTO.rolesToRegister.ToList());

                    if (!roleAvailable)
                        throw new BusinessException("Input role not valid");

                    var roles_to_add = userDTO.rolesToRegister
                        .Where(r => r != null)
                        .Select(r => r.Gcode)
                        .ToList();

                    roles = await UserRoleService.AddUserRole(created_user.Id, roles_to_add, int.Parse(userDTO.MainRoleGcode));
                }
                else
                {
                    roles.Add(await UserRoleService.AddUserRole(created_user.Id, guestRole, isMain: true));
                }

                var exists = roles.Where(r => r.Gcode == int.Parse(userDTO.MainRoleGcode)).Any();

                if (!exists)
                    throw new BusinessException("User MainRole doesn't match selected roles to add");

                #endregion

                await CoreService.CommitAsync();
                await CoreService.CommitTransaction();
                
            }
            catch (Exception)
            {
                await CoreService.RollBackTransaction();
                throw;
            }
        }
    }
}
