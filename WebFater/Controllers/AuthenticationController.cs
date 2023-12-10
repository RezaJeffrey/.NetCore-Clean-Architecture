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

        [HttpPost("SignUp")]
        [AllowAnonymous]
        public async Task<string> Register(AuthDTO user)
        {
            return await AuthenticationService.Register(user);
        }
    }
}
