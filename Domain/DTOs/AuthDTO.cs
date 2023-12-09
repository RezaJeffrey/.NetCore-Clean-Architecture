using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class AuthDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
        public string? PasswordRepeat { get; set; }
        public string? MainRole { get; set; }
        public List<RoleDTO> rolesToRegister { get; set; } = new List<RoleDTO>();
    }
}
