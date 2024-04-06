using CoreLayer.Interfaces;
using CoreLayer.Services;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreLayer.Installers
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
            service.AddScoped(typeof(ICoreService<>), typeof(CoreService<>));
            service.AddScoped(typeof(AuthUcService));

        }
    }
}
