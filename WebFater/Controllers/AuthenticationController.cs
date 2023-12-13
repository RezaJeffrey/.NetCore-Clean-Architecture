using Application.Services;
using Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("UserSignUp")]
        [AllowAnonymous]
        public async Task<string> UserSignUp(AuthDTO user)
        {
            await AuthenticationService.UserSignUp(user);
            return await AuthenticationService.GetAccessToken(user);
        }

        [HttpPost("RegisterUser")]
        [Authorize(Policy = "Policy1")]
        public async Task<string> RegisterUser(AuthDTO user)
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
