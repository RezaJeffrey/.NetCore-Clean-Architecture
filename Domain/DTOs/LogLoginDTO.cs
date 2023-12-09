using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class LogLoginDTO
    {
        public string? IpAddress { get; set; }

        public long? ExpDate { get; set; }

        public bool? IsSuccess { get; set; }
    }
}
