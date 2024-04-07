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
        public long? Id { get; set; }
        public long? ParentId {  get; set; }
        public string? AccessCode {  get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string? PasswordRepeat { get; set; }
        public long? UserStatusId { get; set; }

        // Address
        public long? ProvinceID { get; set; }
        public long? CityID { get; set; }
        public string? Street { get; set; }

        public RoleDTO? Role { get; set; }

    }
}
