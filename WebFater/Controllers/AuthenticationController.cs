using Application.Services;
using Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WebFater.Controllers
{
    [ApiController]
    [Route("Authentication")]
    [Authorize]
    public class AuthenticationController
    {
        private readonly AuthenticationService AuthenticationService;
        public AuthenticationController(AuthenticationService authService)
        {
            AuthenticationService = authService;
        }

        [HttpPost("SignUpUser")]
        [AllowAnonymous]
        public async Task<string> Register(AuthDTO user)
        {
            await AuthenticationService.UserSignUp(user);
            return await AuthenticationService.GetAccessToken(user);
        }

        [HttpPost("SignUpAdmin")]
        [AllowAnonymous]
        public async Task<string> AdminRegister(AuthDTO user)
        {
            await AuthenticationService.RegisterUser(user);
            return await AuthenticationService.GetAccessToken(user);
        }
        
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<string> Login(AuthDTO user)
        {
            return await AuthenticationService.GetAccessToken(user);
        }

    }
}
