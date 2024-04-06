using System;
using System.Globalization;
using Utils.Exceptions;

namespace Utils.Extentions
{
    public static class DateTimeExtensions
    {
        // Convert long to DateTime
        public static DateTime ToMiladiDateTime(this long? dateTimeTicks)
        {
            try
            {
                if (dateTimeTicks == null)
                    return new DateTime(0);

                return new DateTime((long)dateTimeTicks);
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex, ex.Message);
            }
        }

        // Convert Miladi_DateTime to Shamsi_DateTime
        public static string ToShamsiDateTime(this DateTime dateTime)
        {
            try
            {
                PersianCalendar persianCalendar = new PersianCalendar();

                return string.Format(@"{0}/{1}/{2} - {3}:{4}",
                        persianCalendar.GetYear(dateTime),
                        persianCalendar.GetMonth(dateTime),
                        persianCalendar.GetDayOfMonth(dateTime),
                        persianCalendar.GetHour(dateTime),
                        persianCalendar.GetMinute(dateTime)
                    );
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex, ex.Message);
            }
        }

        // Convert long to Shamsi_DateTime
        public static string ToShamsiDateTime(this long? dateTime)
        {
            try
            {
                if (dateTime == null)
                    return string.Empty;

                return dateTime.ToMiladiDateTime().ToShamsiDateTime();
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex, ex.Message);
            }
        }


        // Convert Miladi_ShortDate to Shamsi_ShortDate
        public static string ToShamsiShortDate(this DateTime dateTime)
        {
            try
            {
                PersianCalendar persianCalendar = new PersianCalendar();

                return string.Format(@"{0}/{1}/{2}",
                        persianCalendar.GetYear(dateTime),
                        persianCalendar.GetMonth(dateTime),
                        persianCalendar.GetDayOfMonth(dateTime)
                    );
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex, ex.Message);
            }
        }

        public static string? ToShamsiShortDate(this long? dateTime)
        {
            try
            {
                if (dateTime == null)
                    return null;

                return dateTime.ToMiladiDateTime().ToShamsiShortDate();
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex, ex.Message);
            }
        }

        // Convert Shamsi_Date_String to Milasdi_Date_String 
        public static string ToMiladiDateFromShamsi(this string? shamsiDate)
        {
            try
            {
                if (string.IsNullOrEmpty(shamsiDate))
                    return string.Empty;

                PersianCalendar persianCalendar = new PersianCalendar();
                var sliced = shamsiDate.Split('/');
                return new DateTime
                (
                    int.Parse(sliced[0]),  // Year
                    int.Parse(sliced[1]),  // Month
                    int.Parse(sliced[2].Substring(0, 2)),  // Day
                    persianCalendar
                )
                  .ToString();
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex, ex.Message);
            }
        }

        public static string ToMiladiDateTimeFromShamsi(this string? shamsiDate)
        {
            try
            {
                if (string.IsNullOrEmpty(shamsiDate))
                    return string.Empty;

                PersianCalendar persianCalendar = new PersianCalendar();
                var sliced = shamsiDate.Split('/');
                var slicedTime = sliced[2].Substring(4).Split(":");
                return new DateTime
                (
                    int.Parse(sliced[0]),  // Year
                    int.Parse(sliced[1]),  // Month
                    int.Parse(sliced[2].Substring(0, 2)),  // Day
                    int.Parse(slicedTime[0]),  // Hour
                    int.Parse(slicedTime[1]),  // Minute
                    0,  // Seconds
                    persianCalendar
                )
                  .ToString();
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex, ex.Message);
            }
        }
    }
}
