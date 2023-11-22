using Domain.CoreServices;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Domain.CoreServices;
namespace WebFater.Installers
{
    public static class DomainServiceInstaller
    {
        public static void UseDbContext(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<EcommerceDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DevConnection"));
                }
            );

            service.AddScoped<DbContext, EcommerceDbContext>();
            service.AddScoped(typeof(CoreService<>));
            
        }
    }
}
