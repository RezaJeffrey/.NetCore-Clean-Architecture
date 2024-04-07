using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public partial class CleanContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            // Auth models
            modelBuilder.Entity<User>().HasQueryFilter(u => u.DeleteDate == null || u.DeleteDate == 0);
            modelBuilder.Entity<Role>().HasQueryFilter(u => u.DeleteDate == null || u.DeleteDate == 0);
            modelBuilder.Entity<LogLogin>().HasQueryFilter(u => u.DeleteDate == null || u.DeleteDate == 0);

            // common models
            //modelBuilder.Entity<File>().HasQueryFilter(u => u.DeleteDate == null || u.DeleteDate == 0);


        }
    }
}
