using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Web.Installers
{
    public static class SwaggerConfiguration
    {
        public static void ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddSwaggerGen(c => {
            //    c.AddSecurityDefinition("Bearer",
            //            new ApiKeyScheme
            //            {
            //                In = "header",
            //                Description = "Please enter into field the word 'Bearer' following by space and JWT",
            //                Name = "Authorization",
            //                Type = "apiKey"
            //            });
            //        c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
            //            { "Bearer", Enumerable.Empty<string>() },
            //        });

            //});

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Fiber Optic",
                    Version = "v1",
                });

                var xmlPath = Path.Combine(System.AppContext.BaseDirectory, "ApiDocs.xml");
                if (Path.DirectorySeparatorChar == '/')
                {
                    xmlPath = Path.Combine(Directory.GetCurrentDirectory(), "ApiDocs.xml");
                }
                config.IncludeXmlComments(xmlPath);

                config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                config.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    { 
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                              Scheme = "oauth2",
                              Name = "Bearer",
                              In = ParameterLocation.Header,
                        },
                            new List<string>()
                    }
                });
            });
        }
    }
}
