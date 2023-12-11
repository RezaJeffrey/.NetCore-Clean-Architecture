using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Installers
{
    public static class AppServicesInstaller
    {
        public static void AddApplicationLayerServices(this IServiceCollection service)
        {

            service.AddTransient<TestRoleService, TestRoleService>();
            service.AddTransient<AuthenticationService, AuthenticationService>();
            service.AddTransient<LoginLogService, LoginLogService>();
            service.AddTransient<UserRoleService, UserRoleService>();
        }
    }
}
