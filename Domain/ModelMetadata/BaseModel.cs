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

        public long? Ddate { get; set; }

        public long? DuserId { get; set; }

        public long? Cdate { get; set; }

        public long? CuserId { get; set; }
    }
}
