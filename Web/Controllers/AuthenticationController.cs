using Application.Services;
using CoreLayer.Installers.AuthConfig.Policies;
using Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;
using Utils.Exceptions;
using Utils.Extentions;
using Utils.Statics;

namespace Web.Controllers
{
    [ApiController]
    [Route("Authentication")]
    [Authorize]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService AuthenticationService;
        private readonly ILogger<AuthenticationController> _logger;
        public AuthenticationController(
            AuthenticationService authService,
            ILogger<AuthenticationController> logger)
        {
            AuthenticationService = authService;
            _logger = logger;
        }

        //[HttpPost("UserSignUp")]
        //[AllowAnonymous]
        //public async Task<string> UserSignUp(AuthDTO user)
        //{
        //    await AuthenticationService.UserSignUp(user);
        //    return await AuthenticationService.GetAccessToken(user);
        //}

        /// <summary>
        ///     Admin create new User
        /// </summary>
        /// <remarks>
        ///     <strong>Role: </strong> Admin <br/>
        ///     <strong>Description: </strong> ساخت کاربر جدید <br/>
        /// Fields: <br/>
        /// <para>
        /// {
        ///     <ul>
        ///         <li><strong>"parentId":</strong> 0, آیدی کاربر بالادستی -  (Nullable) درصورت خالی نبودن, اگر نقش کاربر زیرمجموعه پرنت نباشد ارور نمایش داده خواهد شد</li>
        ///         <li><strong>"userName":</strong> "string", نام کاربری - اگر خالی فرستاده شود فیلد شماره همراه بجای آن پر خواهد شد (Nullable)</li>
        ///         <li><strong>"firstName":</strong> "string", </li>
        ///         <li><strong>"lastName":</strong> "string", </li>
        ///         <li><strong>"phoneNumber":</strong> "string", شماره همراه کاربر </li>
        ///         <li><strong>"provinceID":</strong> "string", آیدی استان کاربر</li>
        ///         <li><strong>"cityID":</strong> "string", آیدی شهر کاربر </li>
        ///         <li><strong>"street":</strong> "string", اطلاعات اضافی درباره محل سکونت کاربر مانند خیابان و ... و می تواند خالی باشد </li>
        ///         <li>
        ///             <strong>"status":</strong> {
        ///             <ul>
        ///                 <li>
        ///                     <strong>"gkey":</strong> 0, وضعیت فعلی کاربر - درصورتی که خالی باشد پیش فرض غیرفعال خواهد بود
        ///                     <ul>
        ///                         <li>1: فعال</li>
        ///                         <li>2: غیر فعال</li>
        ///                     </ul>
        ///                 </li>
        ///             </ul>
        ///         </li>
        ///         <li>
        ///             <strong>"Role":</strong> {
        ///             <ul>
        ///                 <li>
        ///                     <strong>"gCode":</strong> 0, کد نقش کاربر
        ///                     <ul>
        ///                         <li>1: Super Admin</li>
        ///                         <li>3: ادمین</li>
        ///                         <li>4: بازاریاب</li>
        ///                         <li>5: بنکدار</li>
        ///                         <li>6: نماینده</li>
        ///                         <li>7: پیمانکار</li>
        ///                         <li>8: خریدار</li>
        ///                     </ul>
        ///                 </li>
        ///             </ul>
        ///         </li>
        ///     </ul>
        /// }
        /// </para>
        /// </remarks>
        /// <param name="user"> AuthDTO</param>
        /// <returns></returns>
        [HttpPost("RegisterUser")]
        [Authorize(Policy = Policy.Admin)]
        public async Task<IActionResult> RegisterUser(AuthDTO user)
        {
            try
            {
                await AuthenticationService.RegisterUser(user);
                return Ok("کاربر با موفقیت ایجاد شد");
            }
            catch(ServiceException ex)
            {
                ex.AddItems(("UserDTO", user));
                throw ex;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, $"errorMessage: {ex.Message} \n " +
                    $"UserInput: {JsonConvert.SerializeObject(user)}");
                throw;
            }

        }
        
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<string> Login(AuthDTO user)
        {
            return await AuthenticationService.GetAccessToken(user);
        }

        /// <summary>
        ///     Send Register Request 
        /// </summary>
        /// <remarks>
        /// Role: Anonymous <br/>
        /// <strong>Description: </strong> Request will be sent to admin or requested parent to Approve or Deny <br/>
        /// <strong>Fields: </strong> <br/>
        /// Fields: <br/>
        /// <para>
        /// {
        ///     <ul>
        ///         <li><strong>"firstName":</strong> "string", First name of the user (Not Required but will be used)</li>
        ///         <li><strong>"lastName":</strong> "string", Last name of the user (Not Required but will be used)</li>
        ///         <li><strong>"phoneNumber":</strong> "string", Phone number of the user (Required)</li>
        ///         <li><strong>"accessCode":</strong> "string", Access code of the user. Null if request role is Vendor or Marketer (Required)</li>
        ///         <li><strong>"provinceID":</strong> 0, ID of the province (Required)</li>
        ///         <li><strong>"cityID":</strong> 0, ID of the city (Required)</li>
        ///         <li><strong>"address":</strong> "string", Address of the user (Not Required but will be used)</li>
        ///         <li><strong>"description":</strong> "string", Description of the user (Not Required but will be used)</li>
        ///         <li>
        ///             <strong>"requestRoleCode":</strong> "0", Role code of the user, where:
        ///             <ul>
        ///                 <li>4: "Marketer"</li>
        ///                 <li>5: "Vendor"</li>
        ///                 <li>6: "Agent"</li>
        ///                 <li>7: "Contractor"</li>
        ///                 <li>8: "Customer"</li>
        ///             </ul>
        ///         </li>
        ///     </ul>
        /// }
        /// </para>
        /// </remarks>
        /// <param name="user"> RegisterRequestDTO </param>
        /// <returns>Message</returns>
        [HttpPost("RegisterRequest")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterRequest(RegisterRequestDTO user)
        {
            try
            {
                await AuthenticationService.RegisterRequest(user);
                return Ok("درخواست با موفقیت انجام شد");
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///     Bind User Register Requests
        /// </summary>
        /// <remarks>
        /// <strong>Role: </strong> Vendor - Agent <br/>
        /// <strong>Description:
        ///     </strong> Vendor or Agent are only able to see the requests with RequestCode of their own AccessCode <br/>
        ///                                 But Admin Can see all the requests<br/>
        /// </remarks>
        /// <param name="gridData"> GridData(RegisterRequestDTO) </param>
        /// <returns>GridData(RegisterRequestDTO)</returns>
        [HttpGet("RequestsBindGrid")]
        [Authorize(Policy = Policy.VendorOrAgent)]
        public async Task<GridData<RegisterRequestDTO>> RequestsBindGrid([FromQuery]GridData<RegisterRequestDTO> gridData)
        {
            return
                await AuthenticationService.BindRegisterRequestsPaging(gridData);
        }


        /// <summary>
        ///     Bind User Register Requests
        /// </summary>
        /// <remarks>
        /// Role: Vendor - Agent <br/>
        /// <strong>Description:
        ///     </strong> Vendor or Agent are only able to Approve or Deny the requests with RequestCode of their own AccessCode <br/>
        ///                                 But Admin Can Review all the requests and has permission to both operations<br/>
        /// 
        /// Fields: <br/>
        /// <para>
        /// {
        ///     <ul>
        ///         <li><strong>"requestId":</strong> 0</li>
        ///         <li>
        ///             <strong>"status":</strong> {
        ///             <ul>
        ///                 <li>
        ///                     <strong>"gkey":</strong> 0, Status key for the project, where:
        ///                     <ul>
        ///                         <li>1: Approve</li>
        ///                         <li>4: Deny</li>
        ///                     </ul>
        ///                 </li>
        ///             </ul>
        ///         </li>
        ///     </ul>
        /// }
        /// </para>
        /// </remarks>
        /// <param name="user"> RegisterRequestDTO </param>
        /// <returns>Message</returns>
        [HttpPost("ApproveUser")]
        [Authorize(Policy = Policy.VendorOrAgent)]
        public async Task<IActionResult> ApproveUser(RegisterRequestDTO user)
        {
            try
            {
                await AuthenticationService.ApproveUser(user);
                switch (user.Status?.Gkey)
                {
                    case 1:
                        return Ok("درخواست تایید شد");

                    case 4:
                        return Ok("درخواست رد شد");

                    case 3:
                        return Ok("درخواست به وضعیت بررسی نشده تغییر کرد");

                    default:
                        throw new ServiceException("وضعیت انتخابی معتبر نمی باشد");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///     Get List of Users 
        /// </summary>
        /// <remarks>
        /// Role: Admin <br/>
        /// </remarks>
        /// <param name="gridData"></param>
        /// <returns></returns>
        [HttpGet("BindUsersPaging")]
        [Authorize(Policy = Policy.Admin)]
        public async Task<GridData<UserDTO>> BindUsersPaging([FromQuery]GridData<UserDTO> gridData)
        {
            return await AuthenticationService.BindUsersPaging(gridData);
        }

        /// <summary>
        ///     Edit User Informations
        /// </summary>
        /// <remarks>
        ///     <strong>Role: </strong> Authorize <br/>
        ///     <strong>Description: </strong> Update User Informations : 
        ///         (UserName, FirstName, LastName, PhoneNumber) <br/>
        ///         درصورت ارسال فیلد آیدی که مربوط به آیدی کاربر می باشد, میتوان کاربر مورد نظر را ویرایش کرد که نیازمند دسترسی می باشد. 
        ///         <br/>
        ///          ادمین دسترسی کامل به ویرایش وضعیت کاربر دارد اما نماینده و بنکدار صرفا می توانند اطلاعات زیر مجموعه خود را ویرایش کنند.<br/>
        /// Fields: <br/>
        /// <para>
        /// {
        ///     <ul>
        ///         <li><strong>"Id":</strong> 0, آیدی کاربری انتخابی جهت ویرایش</li>
        ///         <li><strong>"userName":</strong> 0, آیدی کاربری انتخابی جهت ویرایش</li>
        ///         <li><strong>"firstName":</strong> 0, آیدی کاربری انتخابی جهت ویرایش</li>
        ///         <li><strong>"lastName":</strong> 0, آیدی کاربری انتخابی جهت ویرایش</li>
        ///         <li><strong>"phoneNumber":</strong> 0, آیدی کاربری انتخابی جهت ویرایش</li>
        ///         <li>
        ///             <strong>"status":</strong> {
        ///             <ul>
        ///                 <li>
        ///                     <strong>"gkey":</strong> 0, Status key for the project, where:
        ///                     <ul>
        ///                         <li>1: فعال</li>
        ///                         <li>2: غیر فعال</li>
        ///                     </ul>
        ///                 </li>
        ///             </ul>
        ///         </li>
        ///     </ul>
        /// }
        /// </para>
        /// </remarks>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("EditUser")]
        [Authorize]
        public async Task<UserDTO> EditUser(UserDTO user)
        {
            return await AuthenticationService.EditUser(user);
        }

        /// <summary>
        ///  Delete user
        /// </summary>
        /// <remarks>
        ///     <strong>Role: </strong> Authorize <br/>
        ///     <strong>Description: </strong> کاربر می تواند زیر مجموعه های خود را حذف کند. مثال: بنکدار می تواند نماینده یا مشتری زیرمحموعه خودش را حذف کند <br/>
        /// </remarks>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteUser")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(long UserId)
        {
            try
            {
                await AuthenticationService.DeleteUser(UserId);
                return Ok("کاربر با موفقیت حذف شد");
            }
            catch(Exception)
            {
                throw;
            }

        }

    }
}
