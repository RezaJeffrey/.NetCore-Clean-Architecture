using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class ObjectStatusDTO
    {
        public long ObjectId {  get; set; }

        public int? StatusCode { get; set; }    

        public string? StatusMessage { get; set; }
    }
}
