using CoreLayer.Extensions;
using CoreLayer.Interfaces;
using CoreLayer.Services;
using Domain.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Utils.Exceptions;
using Utils.Extentions;
using Utils.Mappings;
using Utils.Services;
using Utils.Statics;

namespace Application.Services
{
    public class AuthenticationService
    {
        private readonly ICoreService<User> CoreService;
        private readonly AuthUcService AuthUcService;
        private readonly AuthUtilService AuthUtilService;
        private readonly RoleService _roleService;
        private readonly LoginLogService LoginLogService;
        private readonly UserRoleService UserRoleService;
        public AuthenticationService(
            AuthUcService authUcService,
            ICoreService<User> coreService,
            AuthUtilService authUtilService,
            RoleService roleService,
            LoginLogService loginLogService,
            UserRoleService userRoleService
            )
        {
            CoreService = coreService;
            AuthUcService = authUcService;
            AuthUtilService = authUtilService;
            _roleService = roleService;
            LoginLogService = loginLogService;
            UserRoleService = userRoleService;
        }
        
        public async Task<string> GetAccessToken(AuthDTO user_dto)
        {
            var log = new LogLogin();
            User? user = null;
            try
            {
                user_dto.UserName = 
                    (string.IsNullOrWhiteSpace(user_dto.UserName))
                        ? (user_dto.PhoneNumber ?? throw new ServiceException(" حداقل یکی از فیلد های شماره تلفن یا نام کاربری باید وارد شود"))
                        : user_dto.UserName;

                if (string.IsNullOrWhiteSpace(user_dto.PhoneNumber))
                {
                    user_dto.PhoneNumber = user_dto.UserName;
                }

                user = await CoreService.Table()
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.UserName == user_dto.UserName || u.PhoneNumber == user_dto.PhoneNumber);

                if (user == null)
                    throw new ServiceException("نام کاربری یا رمز عبور اشتباه است");


                var Now = DateTime.Now.Ticks;
                var logs = CoreService.Table<LogLogin>()
                    .Where(l => l.UserId == user.Id && l.ExpDate > Now && !l.IsSuccess);

                log = new LogLogin
                {
                    UserId = user.Id,
                    IpAddress = AuthUtilService.GetClientIp(),
                    ExpDate = DateTime.Now.AddDays(1).Ticks
                };

                if (logs.Count() > 20)
                    throw new ServiceException(
                            "حساب کاربری شما به دلیل ورود اشتباه بسیار, برای 24 ساعت غیرفعال شده است"
                        );


                bool passwordMatch = await AuthUtilService
                    .ValidatePassword(user_dto.Password, user.PasswordSalt, user.PasswordHash);
            
                if (!passwordMatch)
                {
                    throw new ServiceException("نام کاربری یا رمز عبور اشتباه است");
                }


                var userRoles = user.UserRoles.Select(ur => ur.Role).ToList();

                bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

                bool isSuperAdmin = false;
                if (userRoles.Any(r => r.Gcode == 1) && isDevelopment)
                {
                    isSuperAdmin = true;
                }
                var token = AuthUcService.CreateToken(user, userRoles, isSuperAdmin);

                log.IsSuccess = true;
                await LoginLogService.CoreService.Create(log);


                return token;
            }
            catch (Exception)
            {
                // Insert log
                log.IsSuccess = false;
                if (user != null) await LoginLogService.CoreService.Create(log);

                throw;
            }
        }

        public async Task<User> CreateUser(AuthDTO userDTO, bool save = true)
        {
            if (string.IsNullOrWhiteSpace(userDTO.PhoneNumber))
            {
                throw new ServiceException("فیلد شماره همراه اجباری است");
            }

            if (userDTO.Password.IsNullOrEmpty())
                throw new ServiceException("مقدار ورودی اشتباه است");

            if (userDTO.Password != userDTO.PasswordRepeat)
                throw new ServiceException("تکرار رمز عبور نادرست است");

            bool userExists = CoreService.Table()
                .FirstOrDefault(user => user.UserName == userDTO.UserName) != null;

            if (userExists)
                throw new ServiceException(
                        "کاربری با این مشخصات وجود دارد, لطفا وارد شوید یا حساب جدیدی بسازید"
                    );

            var RegisterUser = ObjectMapper.MapObject<AuthDTO, User>(userDTO);

            var hashService = await 
                AuthUtilService.HashPassword(userDTO.Password ?? 
                    throw new ServiceException("فیلد رمز عبور اجباری است"));

            RegisterUser.PasswordHash = hashService.hash;
            RegisterUser.PasswordSalt = hashService.salt;
            RegisterUser.PhoneNumber = userDTO.PhoneNumber;
            //RegisterUser.UserStatusId = userDTO.UserStatusId;
            //RegisterUser.AccessCode = userDTO.AccessCode;

            //if (userDTO.ParentId != null && userDTO.ParentId != 0)
            //{
            //    var parent = await CoreService.Table().FirstOrDefaultAsync(u => u.Id == userDTO.ParentId)
            //        ?? throw new ServiceException("Parent Not Valid");

            //    RegisterUser.ParentId = parent.Id;  
            //}

            await CoreService.Create(RegisterUser, save);
            return RegisterUser;
        }

        public async Task<GridData<UserDTO>> BindUsersPaging(GridData<UserDTO> gridData)
        {
            var UsersGrid = await CoreService.Table()
                //.Include(user => user.UserStatus)
                .Include(user => user.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToPagingGridAsync<User, UserDTO>(gridData.pageNumber, gridData.pageSize);

            if (UsersGrid.Data.Count == 0)
            {
                return new GridData<UserDTO>();
            }

            var paginated = UsersGrid.Data.Select(user => new UserDTO()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                AccessCode = user.AccessCode,
                //StatusString = user.UserStatus?.Name ?? "نامشخص",
                UserRole = user.UserRoles.FirstOrDefault(ur => ur.IsMainRole)?.Role.Name ?? "نامشخص",
                CreateDate = user.CreateDate.ToShamsiShortDate() ?? "N/A"
            })
                .ToList();

            UsersGrid.GridData.Data = paginated;
            return UsersGrid.GridData;
        }

        public async Task DeleteUser(long userId)
        {
            var Creds = AuthUtilService.GetUserCredentials();

            if (!Creds.UserRole.IsAdminOrSuperAdmin())
            {
                // Check Conditions
                if (Creds.UserRole == "5" || Creds.UserRole == "6")
                {
                    var SubUsers = await _userService.BindSubUsers(userId);
                    if (!SubUsers.Any(u => u.Id == userId))
                    {
                        throw new ServiceException("شما دسترسی کافی برای انجام این عملیات را ندارید");
                    }
                }
                else
                {
                    throw new ServiceException("شما دسترسی کافی برای انجام این عملیات را ندارید");
                }
            }

            // Delete User
            var User =
                await
                    CoreService.Table()
                    .FirstOrDefaultAsync(u => u.Id == userId)
                        ?? throw new ServiceException("کاربر یافت نشد");

            await CoreService.Delete(User);
        }

    }
}
