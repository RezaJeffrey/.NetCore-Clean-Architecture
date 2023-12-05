using Application.Services;
using Utils.Services;

namespace WebFater.Installers
{
    public static class UtilServicesInstaller
    {
        public static void AddUtilityServices(this IServiceCollection service)
        {

            service.AddAutoMapper(typeof(Program));
            service.AddScoped(typeof(AuthUtilService));
        }
    }
}
