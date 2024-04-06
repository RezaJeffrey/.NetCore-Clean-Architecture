using CoreLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Exceptions;

namespace CoreLayer.Extensions
{
    public static class StringExtensions
    {
        
        public static bool IsAdminOrSuperAdmin(this string role)
        {
            var Roles = new List<int>()
            {
                1, 3, 4, 5, 6, 7, 8, 10
            };

            if (int.TryParse(role, out int Role))
            {
                if (!Roles.Any(r => r == Role))
                {
                    throw new ServiceException("Invalid Role");
                }
                if (Role == 1 || Role == 3)
                    return true;

                return false;
            }
            return false;
        }
    }
}
