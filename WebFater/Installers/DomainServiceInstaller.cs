using CoreLayer.Interfaces;
using CoreLayer.Services;
using Domain.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
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


            //service.AddScoped<ICoreService<User, UserDTO>, CoreService<User, UserDTO>>();
            //service.AddScoped<ICoreService<Role, RoleDTO>, CoreService<Role, RoleDTO>>();
            //service.AddScoped(typeof(CoreService<,>));

        }
    }
}
