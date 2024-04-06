using CoreLayer.Installers.AuthConfig.Handlers;
using CoreLayer.Installers.AuthConfig.Policies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CoreLayer.Installers.AuthConfig
{
    public static class ConfigAuthentication
    {
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var signInKey = Encoding.UTF8.GetBytes(
                    configuration.GetSection("AppSettings:TokenKey").Value ?? string.Empty
                );

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        IssuerSigningKey = new SymmetricSecurityKey(signInKey),
                        ValidateLifetime = true,
                    };

                });

            // Add policies
            services.AddPolicies();

            services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();
        }
    }
}
