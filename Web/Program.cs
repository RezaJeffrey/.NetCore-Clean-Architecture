using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Serilog;
using Web.Installers;
using Web.Middlewares;


var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Host.UseSerilog((context, loggerConfig) => 
    loggerConfig.ReadFrom.Configuration(context.Configuration));


var staticFilesPath = Path.Combine(builder.Environment.ContentRootPath, "StaticFiles");
if (!Directory.Exists(staticFilesPath))
{
    Directory.CreateDirectory(staticFilesPath);
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});



// Install service Extensions
builder.Services.InstallServices(builder.Configuration);



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

// app.UseHttpsRedirection();

app.UseMiddleware<RequestLogContextMiddleware>();
app.UseSerilogRequestLogging();

app.UseStaticFiles();

app.UseCors("DevPolicies");
app.UseAuthentication();
app.UseAuthorization();

var test = builder.Environment.ContentRootPath;
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           staticFilesPath
       ),
    RequestPath = "/StaticFiles"
});

app.MapControllers();

app.Run();
