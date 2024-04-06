using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Exceptions
{
    public class ServiceException : Exception
    {
        public int StatusCode { get; set; } = (int)HttpStatusCode.BadRequest;
        public Dictionary<object, object> Items { get; set; } = new Dictionary<object, object>();

        #region Constructor
        public ServiceException() : base("application rule violation error") { }

        public ServiceException(string message) : base(message) { }

        public ServiceException(Exception exception, string message) : base(message, exception) { }
        public ServiceException(string message, int code) : base(message)
        {
            StatusCode = code;
        }

        public ServiceException(string message, Dictionary<object, object> items, int? code = null) : base(message)
        {
            StatusCode = code ?? (int) HttpStatusCode.BadRequest;
            Items = items;
        }
        #endregion

        public ServiceException AddItems(params (object, object)[] keyValuePairs)
        {
            foreach (var (key, value) in keyValuePairs)
            {
                Items[key] = value;
            }
            return this;
        }
    }
}
