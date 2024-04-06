using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Installers
{
    public static class AppServicesInstaller
    {
        public static void AddApplicationLayerServices(this IServiceCollection service)
        {

            service.AddTransient<RoleService, RoleService>();
            service.AddTransient<AuthenticationService, AuthenticationService>();
            service.AddTransient<LoginLogService, LoginLogService>();
            service.AddTransient<UserRoleService, UserRoleService>();
            service.AddTransient<UserService, UserService>();
            service.AddTransient<ProjectService, ProjectService>();
            service.AddTransient<FinanceService, FinanceService>();
            service.AddTransient<ApplicationService, ApplicationService>();
            service.AddTransient<LocationService, LocationService>();
            service.AddTransient<ServiceTaskService, ServiceTaskService>();
            service.AddTransient<ServiceProjectService, ServiceProjectService>();
            service.AddTransient<WorkFlowService, WorkFlowService>();
            service.AddTransient<FileService, FileService>();
            service.AddTransient<MessageService, MessageService>();
            service.AddTransient<SubjectService, SubjectService>();
            service.AddTransient<TicketService, TicketService>();
            service.AddTransient<TicketMessageService, TicketMessageService>();
            service.AddTransient<UserLocationService, UserLocationService>();
            service.AddTransient<CategoryService, CategoryService>();
            service.AddTransient<ProductService, ProductService>();
            service.AddTransient<OrderService, OrderService>();
        }
    }
}
