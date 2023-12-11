using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Exceptions
{
    public class BusinessException : Exception
    {
        public int StatusCode { get; set; } = (int)HttpStatusCode.BadRequest;
        #region Constructor
        public BusinessException() : base("application rule violation error") { }
        public BusinessException(string message) : base(message) { }
        public BusinessException(string message, int code) : base(message)
        {
            StatusCode = code;
        }
        #endregion
    }
}
