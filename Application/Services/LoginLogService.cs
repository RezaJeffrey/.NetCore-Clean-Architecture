using CoreLayer.Interfaces;
using Domain.DTOs;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LoginLogService
    {
        public ICoreService<LogLogin, LogLoginDTO> CoreService { get; set; }
        public LoginLogService(ICoreService<LogLogin, LogLoginDTO> coreService)
        {
            CoreService = coreService;
        }
    }
}
