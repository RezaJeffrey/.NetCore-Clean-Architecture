using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Exceptions
{
    public class AppRuleException : Exception
    {
        public int StatusCode { get; set; } = (int)HttpStatusCode.BadRequest;
        #region Constructor
        public AppRuleException() : base("application rule violation error") { }
        public AppRuleException(string message) : base(message) { }
        public AppRuleException(string message, int code) : base(message)
        {
            StatusCode = code;
        }
        #endregion
    }
}
