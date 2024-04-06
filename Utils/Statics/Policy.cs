using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Statics
{
    /// <summary>
    ///  Gcode of Defined Roles
    /// </summary>
    public static class Policy
    {
        /// <summary>
        ///     Super Admin
        /// </summary>
        public const string SuperAdmin = "1";
        /// <summary>
        ///     پشتیبان - Admin
        /// </summary>
        public const string Admin = "3";
        /// <summary>
        ///     بازاریاب - Marketer
        /// </summary>
        public const string Marketer = "4";
        /// <summary>
        ///     بنکدار - Vendor
        /// </summary>
        public const string Vendor = "5";
        /// <summary>
        ///     نماینده/ کارگزار - Agent
        /// </summary>
        public const string Agent = "6";
        /// <summary>
        ///     پیمانکار - Contractor
        /// </summary>
        public const string Contractor = "6";
        /// <summary>
        /// (خریدار - (مالک ساختمان قدیم
        /// </summary>
        public const string Customer = "8";


        //public const string Guest = "10";

        #region Or
        public const string MarketerOrAgent = Marketer + Agent;

        public const string VendorOrAgent = Vendor + Agent;

        public const string CustomerOrAgent = Customer + Agent;

        public const string CustomerOrMarketer = Customer + Marketer;

        public const string CustomerOrContractor = Customer + Contractor;
        #endregion
    }
}
