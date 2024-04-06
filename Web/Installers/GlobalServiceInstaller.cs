using CoreLayer.Installers.AuthConfig.Policies;
using CoreLayer.Interfaces;
using CoreLayer.Services;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Web.Installers
{
    public static class GlobalServiceInstaller
    {
        public static void AddGlobalServices(this IServiceCollection service, IConfiguration configuration)
        {
            
            service.AddHttpContextAccessor();

            // ADD CORS POLICIES
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();

            service.AddCors(options =>
            {
                options.AddPolicy("DevPolicies", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
        }
    }
}
