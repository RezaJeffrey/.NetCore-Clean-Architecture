using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Extentions;

namespace Domain.DTOs
{
    public class UserDTO
    {
        public long? Id { get; set; }
        public string? UserName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? StatusString { get; set; }

        public string? AccessCode { get; set; }

        public string? UserRole { get; set; }   

        public string? CreateDate { get; set; }

        public List<RoleDTO> Roles { get; set; } = new List<RoleDTO>();

        public StatusDTO? Status { get; set; }

        public static implicit operator UserDTO(User user)
        {
            return new UserDTO()
            {
                Id = user.Id,   
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                StatusString = user.UserStatus?.Name ?? "نامشخص",
                AccessCode = user.AccessCode,
                UserRole = user.UserRoles?.FirstOrDefault(ur => ur.IsMainRole)?.Role?.Name ?? "نامشخص",
                CreateDate = user.CreateDate.ToShamsiShortDate() ?? "N/A",
            };
        }
    }
}
