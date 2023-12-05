using CoreLayer.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebFater.Installers
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
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireRole2", policy => policy.Requirements.Add(new RoleAuthorizationRequirement("2")));
            });

            services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();
        }
    }
}
