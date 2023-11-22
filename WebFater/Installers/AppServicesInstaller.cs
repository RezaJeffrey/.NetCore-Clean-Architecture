using Application.Services;
using Domain.CoreServices;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace WebFater.Installers
{
    public static class AppServicesInstaller
    {
        public static void AddApplicationLayerServices(this IServiceCollection service)
        {
        
            service.AddTransient<TestProductService, TestProductService>();
        }
    }
}
