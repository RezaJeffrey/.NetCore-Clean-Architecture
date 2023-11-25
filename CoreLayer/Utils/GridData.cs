using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Utils
{
    public class GridData<T> where T : class
    {
        public List<T> Data { get; set; } = new List<T>();
        public long pageNumber { get; set; } = 1;
        public long pageSize { get; set; } = long.MaxValue;
        public long totalPages { get; set; }
        public long totalCount { get; set; }
        public List<Filter> filters { get; set; } = new List<Filter>();
    }
}
