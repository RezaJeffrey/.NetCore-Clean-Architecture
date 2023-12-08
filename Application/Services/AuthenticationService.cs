using CoreLayer.Services;
using Domain.DTOs;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthenticationService
    {
        private readonly CoreService<User, UserDTO> CoreService;
        private readonly AuthUcService AuthUcService;
        public AuthenticationService(AuthUcService authUcService, CoreService<User, UserDTO> coreService)
        {
            CoreService = coreService;
            AuthUcService = authUcService;
        }

        public async Task<string> GetAccessToken()
        {
            throw new NotImplementedException();
        }
        
        public async Task<string> RefreshToken()
        {
            throw new NotImplementedException();
        }
        
        public async Task<string> Login()
        {
            throw new NotImplementedException();
        }
        public async Task<string> Register()
        {
            throw new NotImplementedException();
        }

    }
}
