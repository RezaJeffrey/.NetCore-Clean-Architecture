using CoreLayer.Installers.AuthConfig.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Utils.Statics;

namespace CoreLayer.Installers.AuthConfig.Policies
{
    public static class Policies
    {
        public static void AddPolicies(this IServiceCollection service)
        {
            service.AddAuthorization(options =>
            {
                
                options.AddPolicy(
                        Policy.SuperAdmin,
                        policy => policy.Requirements.Add(new RoleAuthorizationRequirement(Policy.SuperAdmin))
                    );
                
                options.AddPolicy(
                        Policy.Admin,
                        policy => policy.Requirements.Add(new RoleAuthorizationRequirement(Policy.Admin))
                    );

                options.AddPolicy(
                        Policy.Marketer,
                        policy => policy.Requirements.Add(new RoleAuthorizationRequirement(Policy.Marketer))
                    );

                options.AddPolicy(
                        Policy.Vendor,
                        policy => policy.Requirements.Add(new RoleAuthorizationRequirement(Policy.Vendor))
                    );

                options.AddPolicy(
                        Policy.Agent,
                        policy => policy.Requirements.Add(new RoleAuthorizationRequirement(Policy.Agent))
                    );

                options.AddPolicy(
                        Policy.Contractor,
                        policy => policy.Requirements.Add(new RoleAuthorizationRequirement(Policy.Contractor))
                    );

                options.AddPolicy(
                        Policy.Customer,
                        policy => policy.Requirements.Add(new RoleAuthorizationRequirement(Policy.Customer))
                    );
                //options.AddPolicy(
                //        Policy.Guest,
                //        policy => policy.Requirements.Add(new RoleAuthorizationRequirement(Policy.Guest))
                //    );


                // Or Policies
                #region Or

                // Marketer or Agent
                options.AddPolicy(  
                        Policy.MarketerOrAgent,
                        policy => 
                            policy.Requirements.Add(
                                    new RoleAuthorizationRequirement(new List<string>()
                                    {
                                        Policy.Agent,
                                        Policy.Marketer
                                    })
                                )
                    );

                // Vendor or Agent
                options.AddPolicy(
                        Policy.VendorOrAgent,
                        policy =>
                            policy.Requirements.Add(
                                    new RoleAuthorizationRequirement(new List<string>()
                                    {
                                        Policy.Agent,
                                        Policy.Vendor
                                    })
                                )
                    );

                // Customer or Agent
                options.AddPolicy(
                        Policy.CustomerOrAgent,
                        policy =>
                            policy.Requirements.Add(
                                    new RoleAuthorizationRequirement(new List<string>()
                                    {
                                        Policy.Agent,
                                        Policy.Customer
                                    })
                                )
                    );

                // Customer or Marketer
                options.AddPolicy(
                        Policy.CustomerOrMarketer,
                        policy =>
                            policy.Requirements.Add(
                                    new RoleAuthorizationRequirement(new List<string>()
                                    {
                                        Policy.Customer,
                                        Policy.Marketer
                                    })
                                )
                    );

                // Customer or Contractor
                options.AddPolicy(
                        Policy.CustomerOrContractor,
                        policy =>
                            policy.Requirements.Add(
                                    new RoleAuthorizationRequirement(new List<string>()
                                    {
                                        Policy.Customer,
                                        Policy.Contractor
                                    })
                                )
                    );

                #endregion
            });

        }
    }
}
