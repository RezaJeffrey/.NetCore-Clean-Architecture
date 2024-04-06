using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Installers.AuthConfig.Handlers
{
    public class RoleAuthorizationRequirement : IAuthorizationRequirement
    {
        public string? RequiredRole { get; set; }

        public List<string> RequiredRoles { get; set; } = new List<string>();
        
        public bool IsMulti {  get; set; } = false;
        public RoleAuthorizationRequirement(string role) 
        {
            RequiredRole = role; 
        }
        public RoleAuthorizationRequirement(List<string> roles)
        {
            IsMulti = true;
            RequiredRoles = roles;
        }
    }
}
