using CoreLayer.Interfaces;
using CoreLayer.Services;
using Domain.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Utils.Services;

namespace WebFater.Installers
{
    public static class DomainServiceInstaller
    {
        public static void UseDbContext(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<FaterTestContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DevConnection"));
                }
            );

            service.AddScoped<DbContext, FaterTestContext>();
            service.AddScoped(typeof(ICoreService<,>), typeof(CoreService<,>));
            service.AddScoped(typeof(AuthUcService));
            // move to another Installer Dir
            service.AddSingleton(typeof(IHttpContextAccessor), typeof(HttpContextAccessor));


            //service.AddScoped<ICoreService<User, UserDTO>, CoreService<User, UserDTO>>();
            //service.AddScoped<ICoreService<Role, RoleDTO>, CoreService<Role, RoleDTO>>();
            //service.AddScoped(typeof(CoreService<,>));

        }
    }
}
