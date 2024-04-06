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
        private readonly UserService _userService;
        private readonly UserLocationService _userLocationService;
        public AuthenticationService(
            AuthUcService authUcService,
            ICoreService<User> coreService,
            AuthUtilService authUtilService,
            RoleService roleService,
            LoginLogService loginLogService,
            UserRoleService userRoleService,
            UserService userService,
            UserLocationService userLocationService
            )
        {
            CoreService = coreService;
            AuthUcService = authUcService;
            AuthUtilService = authUtilService;
            _roleService = roleService;
            LoginLogService = loginLogService;
            UserRoleService = userRoleService;
            _userService = userService;
            _userLocationService = userLocationService;
        }
        
        public async Task<string> RefreshToken(string? accessToken = null)
        {
            var deserialized_token = (accessToken.IsNullOrEmpty())
                                        ? AuthUtilService.getClaims()
                                        : AuthUtilService.getClaims(accessToken);   

            long userId = long.Parse(deserialized_token.GetClaim("UserId").Value);

            User? user = await CoreService.Table()
                .Include(user => user.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(user => user.Id == userId);
            
            if (user == null)
                throw new ServiceException("کاربر یافت نشد");

            var userRoles = user.UserRoles.Select(ur => ur.Role).ToList();

            return AuthUcService.CreateToken(user, userRoles);
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
                    .Include(user => user.UserStatus)
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

                if (user.UserStatus?.Gkey != null && user.UserStatus?.Gkey == 2)
                {
                    throw new ServiceException("حساب کاربری شما فعال نشده است. لطفا منتظر بمانید یا با اپشتیبانی در تماس باشید.");
                }

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
            RegisterUser.UserStatusId = userDTO.UserStatusId;
            RegisterUser.AccessCode = userDTO.AccessCode;

            if (userDTO.ParentId != null && userDTO.ParentId != 0)
            {
                var parent = await CoreService.Table().FirstOrDefaultAsync(u => u.Id == userDTO.ParentId)
                    ?? throw new ServiceException("Parent Not Valid");

                RegisterUser.ParentId = parent.Id;  
            }

            await CoreService.Create(RegisterUser, save);
            return RegisterUser;
        }

        public async Task<User?> GetUserByUsername(string username) 
        {
            return await CoreService.Table()
                .FirstOrDefaultAsync(
                        user => user.UserName.ToLower() == username.ToLower()
                    );
        }

        //public async Task UserSignUp(AuthDTO userDTO) 
        //{
        //    try
        //    {
        //        await CoreService.BeginTransaction();

        //        await CreateUser(userDTO);
        //        var created_user = await GetUserByUsername(userDTO.UserName);

        //        if (created_user == null)
        //            throw new ServiceException("User Creation Failed");

        //        #region Add Default Guest UserRole
        //        int guest_role = 10;

        //        var guestRole = CoreService.Table<Role>()
        //            .FirstOrDefault(role => role.Gcode == guest_role);

        //        if (guestRole == null)
        //            throw new ServiceException("Role is not valid");

        //        await UserRoleService.AddUserRole(created_user.RequestId, guestRole.Gcode, isMain: true, save: false);

        //        #endregion

        //        await CoreService.CommitAsync();
        //        await CoreService.CommitTransaction();
        //    }
        //    catch (Exception)
        //    {
        //        await CoreService.RollBackTransaction();
        //        throw;
        //    }
        //}
        public async Task RegisterUser(AuthDTO user)
        {
            try
            {
                var (UserId, UserRole) = AuthUtilService.GetUserCredentials();
                await CoreService.BeginTransaction();

                user.Password = user.Password ?? user.PhoneNumber;
                user.PasswordRepeat = user.Password;
                user.UserName = user.UserName ?? user.PhoneNumber;
                user.AccessCode = 
                    (user.Role?.Gcode == null )
                        ? throw new ServiceException("نقش کاربر مشخص نشده است")
                        : await _userService.GenerateAccessCode(user.Role.Gcode);

                #region UserStatus
                // Default Status for new User will be Inactive
                if (user.Status?.Gkey == null)
                {
                    user.Status = new StatusDTO()
                    {
                        Gkey = 2
                    };
                }

                var Status = 
                    await CoreService.Table<Status>()
                        .FirstAsync(s => s.Gkey == user.Status.Gkey);

                user.UserStatusId = Status.Id;
                #endregion


                #region Set User Parent
                if(user is {ParentId: not null and not 0 })
                {
                    var Parent =
                        await
                        CoreService.Table()
                        .Include(p => p.UserRoles)
                            .ThenInclude(ur => ur.Role)
                            .FirstOrDefaultAsync(p => p.Id == user.ParentId)
                                ?? throw new ServiceException("پرنت انتخابی وجود ندارد");

                    var parentCode = Parent.UserRoles.First(p => p.IsMainRole).Role.Gcode;
                    var parentIsValid =
                        await
                            _roleService
                            .CheckParentIsValid(
                                    parentCode,
                                    user.Role.Gcode
                                );
                    if (!parentIsValid)
                    {
                        throw
                            new ServiceException(
                                    $"کاربر با نقش {user.Role?.Gcode} نمیتواند زیرمجموعه پرنت با نقش {parentCode} باشد"
                                )
                                .AddItems(
                                        ("ParentCode: ", parentCode),
                                        ("childCode: ", user.Role?.Gcode ?? 0)
                                    );
                    }

                    // Set user ParentId
                    user.ParentId = Parent.Id;

                }
                #endregion

                var User = await CreateUser(user);

                #region Create UserLocation
                var City = await CoreService.Table<City>()
                    .FirstOrDefaultAsync(c => c.Id == user.CityID)
                        ?? throw new ServiceException("شهر انتخابی نامعتبر است");

                if (City.ProvinceId != user.ProvinceID)
                    throw new ServiceException("استان انتخابی نادرست است");

                //var Location = new Location()
                //{
                //    CityId = City.Id,
                //    Street = user.Street,
                //};

                var userLocation = new UserLocationDTO()
                {
                    Title = "آدرس اولیه",
                    UserId = User.Id,
                    Location = new LocationDTO()
                    {
                        City = new CityDTO()
                        {
                            Id = user.CityID,
                        },
                        Street = user.Street
                    }
                };

                await _userLocationService.CreateUserLocation(userLocation);
                #endregion


                if (user.Role?.Gcode == null)
                {
                    throw new ServiceException("فیلد نقش کاربر الزامی می باشد");
                }

                var Role =
                        await
                            CoreService.Table<Role>()
                            .FirstOrDefaultAsync(r => r.Gcode == user.Role.Gcode)
                                ?? throw new ServiceException("نقش انتخابی معتبر نمی باشد");

                var userRole = new UserRole()
                {
                    UserId = User.Id,
                    RoleId = Role.Id,
                    IsMainRole = true,
                    CreateUserId = UserId,
                    CreateDate = DateTime.Now.Ticks
                };

                CoreService.GetDb<UserRole>().Add(userRole);

                await CoreService.CommitAsync();
                await CoreService.CommitTransaction();

                
            }
            catch (Exception)
            {
                await CoreService.RollBackTransaction();
                throw;
            }
        }

        public async Task RegisterRequest(RegisterRequestDTO dto)
        {
            try
            {

                await CoreService.BeginTransaction();

                var User = await CoreService.Table()
                    .FirstOrDefaultAsync
                        (u => u.PhoneNumber == dto.PhoneNumber || u.UserName == dto.PhoneNumber);

                if (User != null)
                {
                    throw new ServiceException("کاربر با این شماره تلفن وجود دارد, لطفا با وارد شوید یا شماره تلفن جدیدی وارد کنید");
                }

                User = new User()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    PhoneNumber = dto.PhoneNumber,
                    UserName = dto.PhoneNumber ?? string.Empty,
                    UserStatusId = CoreService.Table<Status>().First(s => s.Gkey == 2).Id
                };
                // TODO Location

                #region Location

                var City = CoreService.Table<City>()
                    .First(c => c.Id == dto.CityID);

                if (City.ProvinceId != dto.ProvinceID)
                    throw new ServiceException("استان انتخابی نادرست است");

                var Location = new Location()
                {
                    CityId = City.Id,
                    Street = dto.Address,
                };

                //await CoreService.GetDb<Location>()
                //    .AddAsync(Location);

                #endregion

                var hashService = await
                    AuthUtilService.HashPassword(dto.PhoneNumber ??
                        throw new ServiceException("فیلد شماره تلفن ضروری است"));

                User.PasswordHash = hashService.hash;
                User.PasswordSalt = hashService.salt;

                await CoreService.Create(User);  // Commiting will Create Location for user - Location/address table might change in future so each user will be able to submit multi addresses 


                #region Create UserLocation
                var userLocation = new UserLocationDTO()
                {
                    Title = "آدرس اولیه",
                    UserId = User.Id,
                    Location = new LocationDTO()
                    {
                        City = new CityDTO()
                        {
                            Id = dto.CityID,
                        },
                        Street = dto.Address
                    }
                };

                await _userLocationService.CreateUserLocation(userLocation);

                #endregion


                var Role = await CoreService.Table<Role>()
                    .FirstOrDefaultAsync(r => r.Gcode.ToString() == dto.RequestRoleCode)
                        ?? throw new ServiceException("نقش انتخابی معتبر نمی باشد");

                // Create RegisterRequest
                var registerReq = await CoreService.Table<RegisterRequest>()
                    .Include(rr => rr.Status)
                    .FirstOrDefaultAsync(rr => rr.UserId == User.Id && rr.Status != null && (rr.Status.Gkey == 3 || rr.Status.Gkey == 4) );

                if (registerReq != null)
                    switch (registerReq.Status?.Gkey)
                    {
                        case 3:
                            throw new ServiceException("لطفا منتظر باشید تا درخواست شما بررسی شود");

                        case 4:
                            throw new ServiceException("درخواست عضویت شما رد شده است. لطفا با پیشتیبانی تماس حاصل فرمایید");
                    }

                if (!string.IsNullOrWhiteSpace(dto.AccessCode))
                {
                    var Parent = await CoreService.Table()
                        .AnyAsync(u => u.AccessCode == dto.AccessCode);

                    if (!Parent)
                        throw new ServiceException("کد وارد شده نادرست است");
                }


                var RegisterRequest = new RegisterRequest()
                {
                    UserId = User.Id,
                    RequestCode = (string.IsNullOrWhiteSpace(dto.AccessCode)) ? null : dto.AccessCode,
                    FirstName = dto.FirstName ?? string.Empty,
                    LastName = dto.LastName ?? string.Empty,
                    PhoneNumber = dto.PhoneNumber,
                    Description = dto.Description,
                    StatusId = CoreService.Table<Status>().First(s => s.Gkey == 3).Id,
                    RoleId = Role.Id,

                    CreateDate = DateTime.Now.Ticks,
                    CreateUserId = User.Id
                };

                await CoreService.GetDb<RegisterRequest>()
                    .AddAsync(RegisterRequest);

                await CoreService.CommitAsync();
                await CoreService.CommitTransaction();
            }
            catch (Exception)
            {
                await CoreService.RollBackTransaction();
                throw;
            }

        }

        public async Task ApproveUser(RegisterRequestDTO dto)
        {
            RegisterRequest? registerReq = null;
            try
            {


                var UserId = AuthUtilService.getUserId();
                var UserRole = AuthUtilService.GetUserRole();
                var Now = DateTime.Now.Ticks;

                if (dto.Status == null) throw new ServiceException();

                var Status = await CoreService.Table<Status>().FirstAsync(s => s.Gkey == dto.Status.Gkey);

                registerReq = await CoreService.Table<RegisterRequest>()
                    .Include(rr => rr.User)
                    .Include(rr => rr.Role)
                    .FirstOrDefaultAsync(rr => rr.Id == dto.RequestId)
                        ?? throw new ServiceException("درخواست عضویت یافت نشد");

                var User = registerReq.User;

                User? Parent = null;
                if (!string.IsNullOrWhiteSpace(registerReq.RequestCode))
                {
                    Parent = await CoreService.Table().FirstOrDefaultAsync(u => u.AccessCode == registerReq.RequestCode);
                }

                if (Parent == null)
                {
                    if (!UserRole.IsAdminOrSuperAdmin())
                    {
                        throw new ServiceException("دسترسی مجاز نیست");
                    }

                }
                else
                {
                    if (!UserRole.IsAdminOrSuperAdmin())
                    {
                        if (UserId != Parent.Id)
                            throw new ServiceException("دسترسی مجاز نیست");
                    }
                    User.ParentId = Parent.Id;
                }


                if (dto.Status?.Gkey != null && Status.Gkey == 1 && registerReq.StatusId != Status.Id)  // درخواست پذیرفته شده است
                {
                    //if (dto.RequestRoleCode != registerReq.RoleId.ToString())
                    //    throw new ServiceException("Requeted Role code didn't match with the input one");

                    var Role = registerReq.Role;

                    UserRole userRole = new UserRole()  // Latest decision was to store only the mainRole for user
                    {
                        UserId = registerReq.UserId,
                        RoleId = Role.Id,
                        IsMainRole = true,
                        CreateDate = DateTime.Now.Ticks,
                    };

                    await CoreService.GetDb<UserRole>()
                        .AddAsync(userRole);

                    //registerReq.RequestCode = dto.AccessCode;

                    registerReq.ModifyDate = Now;
                    registerReq.ModifyUserId = UserId;

                    // Assign AccessCode to user
                    User.AccessCode = await _userService.GenerateAccessCode(Role.Gcode);

                }

                User.UserStatusId = Status.Id;
                registerReq.StatusId = Status.Id;

                CoreService.GetDb<RegisterRequest>().Update(registerReq);

                await CoreService.Update(User);
                await CoreService.CommitAsync();
            }
            catch(Exception ex)
            {
                throw new ServiceException(ex.Message)
                    .AddItems(("Role", registerReq?.Role?.Gcode.ToString() ?? string.Empty));
            }
        }

        // Get Register Requests

        public async Task<GridData<RegisterRequestDTO>> BindRegisterRequestsPaging(GridData<RegisterRequestDTO> gridData)
        {
            var UserId = AuthUtilService.getUserId();
            var UserRole = AuthUtilService.GetUserRole();

            if (string.IsNullOrWhiteSpace(UserRole))
            {
                throw new ServiceException("خطا در دریافت نقش کاربر", 500);
            }

            var Requests = await CoreService.Table<RegisterRequest>()
                .Include(rr => rr.User)
                    .ThenInclude(u => u.UserLocations)
                    .ThenInclude(ul => ul.Location)
                    .ThenInclude(l => l.City)
                    .ThenInclude(p => p.Province)
                .Include(rr => rr.Role)
                .Include(rr => rr.Status)
                .Where(rr => rr.Status.Gkey != 1)
                .ToPagingGridAsync<RegisterRequest, RegisterRequestDTO>(gridData.pageNumber, gridData.pageSize);

            
            if (!UserRole.IsAdminOrSuperAdmin())
            {
                var User = await CoreService.Table().FirstAsync(u => u.Id == UserId);
                Requests.Data = Requests.Data.Where(r => r.RequestCode == User.AccessCode).ToList();
            }

            var DTO = Requests.Data.Select(r => 
            {
                var dto = new RegisterRequestDTO();
                var Parent = CoreService
                    .Table()
                    .FirstOrDefault(u => u.AccessCode == r.RequestCode);

                // User related Requested Identities
                dto.RequestId = r.Id;
                dto.UserID = r.User.Id;
                dto.FirstName = r.FirstName;
                dto.LastName = r.LastName;
                dto.PhoneNumber = r.PhoneNumber;


                // Requests
                dto.AccessCode = r.RequestCode;  // requested access code
                dto.RequestRoleCode = r.Role.Gcode.ToString();
                dto.ParentPhoneNumber =
                    ( !string.IsNullOrWhiteSpace(r.RequestCode) && Parent == null)
                        ? throw new ServiceException("Parent not valid")
                        : (Parent?.PhoneNumber ?? "No parent Requested");

                // Status
                dto.Status = new StatusDTO()
                {
                    Id = r.StatusId,
                    Name = r.Status?.Name,
                    Gkey = r.Status?.Gkey
                };

                dto.Description = r.Description;


                // Locations
                var location = r.User?.UserLocations?.FirstOrDefault()?.Location;

                dto.ProvinceID = location?.City.ProvinceId;
                dto.CityID = location?.CityId;
                dto.Address = location?.Street;
                dto.SProvince = location?.City.Province.Name;
                dto.SCity = location?.City.Name;

                long? date = r.CreateDate;
                dto.RegisterDate = date.ToShamsiShortDate();

                return dto;
            })
                .ToList();

            //var dtos = await Task.WhenAll(tasks: DTO);

            Requests.GridData.Data = DTO;

            return Requests.GridData;
        }

        public async Task<GridData<UserDTO>> BindUsersPaging(GridData<UserDTO> gridData)
        {
            // ToDo: Add UserRole 
            var UsersGrid = await CoreService.Table()
                .Include(user => user.UserStatus)
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
                StatusString = user.UserStatus?.Name ?? "نامشخص",
                UserRole = user.UserRoles.FirstOrDefault(ur => ur.IsMainRole)?.Role.Name ?? "نامشخص",
                CreateDate = user.CreateDate.ToShamsiShortDate() ?? "N/A"
            })
                .ToList();

            UsersGrid.GridData.Data = paginated;
            return UsersGrid.GridData;
        }

        public async Task<UserDTO> EditUser(UserDTO dto)
        {
            var Credentials = AuthUtilService.GetUserCredentials();
            var Statuses = await CoreService.Table<Status>().ToListAsync();

            var User = await CoreService.Table()
                .FirstOrDefaultAsync(u => u.Id == dto.Id)
                    ?? throw new ServiceException("کاربر با این ایدی معتبر نمی باشد");

            #region Edit Status
            if (dto.Id != Credentials.UserId)
            {
                
                if (Credentials.UserRole.IsAdminOrSuperAdmin())
                {
                    // Update Status
                    User.UserStatusId =
                        Statuses.FirstOrDefault(s => s.Gkey == dto.Status?.Gkey)?.Id
                            ?? throw new ServiceException("وضعیت انتخابی معتبر نمی باشد");
                }
                else if (Credentials.UserRole == "5" ||  Credentials.UserRole == "6") 
                {

                    var SubUsers = await _userService.BindSubUsers(Credentials.UserId);
                    if (!SubUsers.Any(u => u.Id == dto.Id))
                        throw new ServiceException("شما دسترسی کافی برای ویرایش این کاربر را ندارید");

                    // Update User Status
                    User.UserStatusId =
                        Statuses.FirstOrDefault(s => s.Gkey == dto.Status?.Gkey)?.Id
                            ?? throw new ServiceException("وضعیت انتخابی معتبر نمی باشد");
                }
                else
                {
                    throw new ServiceException("شما دسترسی کافی برای ویرایش این کاربر را ندارید");
                }
                
            }

            #endregion

            #region Update public Constants
            User.UserName = 
                string.IsNullOrWhiteSpace(dto.UserName) 
                    ? User.UserName
                    : dto.UserName;

            User.FirstName = 
                string.IsNullOrWhiteSpace(dto.FirstName)
                    ? User.FirstName
                    : dto.FirstName;

            User.LastName =
                string.IsNullOrWhiteSpace(dto.LastName)
                    ? User.LastName
                    : dto.LastName;

            User.PhoneNumber =
                string.IsNullOrWhiteSpace(dto.PhoneNumber)
                    ? User.PhoneNumber
                    : dto.PhoneNumber;

            #endregion

            await CoreService.Update(User);
            return User;
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
