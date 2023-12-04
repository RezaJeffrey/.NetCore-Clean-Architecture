using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public partial class FaterTestContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<User>().HasQueryFilter(u => u.DeleteDate == null || u.DeleteDate == 0);
            modelBuilder.Entity<Role>().HasQueryFilter(u => u.DeleteDate == null || u.DeleteDate == 0);
            modelBuilder.Entity<RoleParent>().HasQueryFilter(u => u.DeleteDate == null || u.DeleteDate == 0);
            modelBuilder.Entity<UserRole>().HasQueryFilter(u => u.DeleteDate == null || u.DeleteDate == 0);
        }
    }
}
