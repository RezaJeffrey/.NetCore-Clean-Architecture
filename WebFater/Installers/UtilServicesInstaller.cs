using Application.Services;

namespace WebFater.Installers
{
    public static class UtilServicesInstaller
    {
        public static void AddUtilityServices(this IServiceCollection service)
        {

            service.AddAutoMapper(typeof(Program));
        }
    }
}
