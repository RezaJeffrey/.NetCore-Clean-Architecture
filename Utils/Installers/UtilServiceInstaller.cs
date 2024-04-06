using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Services;
using Utils.Mappings;

namespace Utils.Installers
{
    public static class UtilServicesInstaller
    {
        public static void AddUtilityServices(this IServiceCollection service)
        {
            service.AddAutoMapper(typeof(ObjectMapper));
            service.AddScoped(typeof(AuthUtilService));
        }
    }
}
