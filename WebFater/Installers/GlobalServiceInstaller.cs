using CoreLayer.Interfaces;
using CoreLayer.Services;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace WebFater.Installers
{
    public static class GlobalServiceInstaller
    {
        public static void AddGlobalServices(this IServiceCollection service, IConfiguration configuration)
        {
            
            service.AddSingleton(typeof(IHttpContextAccessor), typeof(HttpContextAccessor));
        }
    }
}
