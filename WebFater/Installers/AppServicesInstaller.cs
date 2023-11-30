using Application.Services;

namespace WebFater.Installers
{
    public static class AppServicesInstaller
    {
        public static void AddApplicationLayerServices(this IServiceCollection service)
        {

            service.AddTransient<TestRoleService, TestRoleService>();
            service.AddTransient<AuthenticateService, AuthenticateService>();
        }
    }
}
