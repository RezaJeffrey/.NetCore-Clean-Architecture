using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebFater.Installers;
using WebFater.Middlewares;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
                IssuerSigningKey = new SymmetricSecurityKey(signInKey)
            };
        });

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
