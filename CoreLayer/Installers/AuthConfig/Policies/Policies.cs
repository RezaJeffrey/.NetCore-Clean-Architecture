using CoreLayer.Installers.AuthConfig.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace CoreLayer.Installers.AuthConfig.Policies
{
    public static class Policies
    {
        public static void AddPolicies(this IServiceCollection service)
        {
            service.AddAuthorization(options =>
            {
                
                options.AddPolicy(
                        "Policy1",
                        policy => policy.Requirements.Add(new RoleAuthorizationRequirement("1"))
                    );
                
                options.AddPolicy(
                        "Policy3",
                        policy => policy.Requirements.Add(new RoleAuthorizationRequirement("3"))
                    );
                
                options.AddPolicy(
                        "Policy4",
                        policy => policy.Requirements.Add(new RoleAuthorizationRequirement("4"))
                    );
                
                options.AddPolicy(
                        "Policy5",
                        policy => policy.Requirements.Add(new RoleAuthorizationRequirement("5"))
                    );

            });
        }
    }
}
