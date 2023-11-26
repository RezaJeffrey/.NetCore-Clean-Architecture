using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Models
{
    public class GridData<T> where T : class
    {
        public List<T> Data { get; set; } = new List<T>();
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = int.MaxValue;
        public int totalPages { get; set; }
        public int totalCount { get; set; }
        public List<Filter> filters { get; set; } = new List<Filter>();
    }
}
