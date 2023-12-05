using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebFater.Installers;
using WebFater.Middlewares;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CoreLayer.Handlers;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Install service Extensions
builder.Services
    .UseDbContext(builder.Configuration);

builder.Services.AddApplicationLayerServices();
builder.Services.AddUtilityServices();

// TODO move auth configs to Installers + Add 'AddServices' to Installers to install AllServices 
// Authentication
var signInKey = Encoding.UTF8.GetBytes(
        builder.Configuration.GetSection("AppSettings:TokenKey").Value ?? string.Empty 
    );
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireRole2", policy => policy.Requirements.Add(new RoleAuthorizationRequirement("2")));
});
builder.Services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMiddleware<GlobalDevExceptionMiddleware>();
}
else
{
    app.UseMiddleware<GlobalExceptionMiddleware>(); 
}
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
