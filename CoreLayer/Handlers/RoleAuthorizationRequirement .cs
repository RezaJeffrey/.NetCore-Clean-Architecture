using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Handlers
{
    public class RoleAuthorizationRequirement : IAuthorizationRequirement
    {
        public string RequiredRole { get; set; }
        public RoleAuthorizationRequirement(string role) { RequiredRole = role; }
    }
}
