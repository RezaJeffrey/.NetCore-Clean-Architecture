using Application.Installers;
using CoreLayer.Installers;
using CoreLayer.Installers.AuthConfig;
using Utils.Installers;

namespace Web.Installers
{
    public static class ServicesInstaller
    {
        public static void InstallServices(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddGlobalServices(configuration);
            services.UseDbContext(configuration);
            services.AddUtilityServices();
            services.AddApplicationLayerServices();
            services.ConfigureAuthentication(configuration);
            services.ConfigureSwagger(configuration);
        }
    }
}
