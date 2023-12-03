using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ModelMetadata
{
    public class BaseModel
    {
        [ForeignKey("ID")]
        public long Id { get; set; }

        //public long? CreateDate { get; set; }
        //public long? CreateUserId { get; set; }
        //public long? ModifyDate { get; set; }
        //public long? ModifyUserId { get; set; } 
        //public long? DeleteDate { get; set; }
        //public long? DeleteUserId { get; set; }

    }
}
