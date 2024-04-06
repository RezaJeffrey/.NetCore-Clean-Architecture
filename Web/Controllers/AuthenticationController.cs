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
        
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<string> Login(AuthDTO user)
        {
            return await AuthenticationService.GetAccessToken(user);
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
