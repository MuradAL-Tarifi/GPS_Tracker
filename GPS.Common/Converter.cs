using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Common
{
    public class Converter
    {
        public static int BoolToInt(bool? val)
        {
            if (val.HasValue && val.Value)
                return 1;

            return 0;

        }

        public static decimal? GetDecimal(string val)
        {
            decimal _out;
            if (decimal.TryParse(val, out _out))
                return _out;

            return null;

        }

        public static long? GetLong(string val)
        {
            long _out;
            if (long.TryParse(val, out _out))
                return _out;

            return null;

        }

        public static int? GetInt(string val)
        {
            int _out;
            if (int.TryParse(val, out _out))
                return _out;

            return null;

        }


        /// <summary>
        /// convert Gregorian string to DateTime Object
        /// </summary>
        /// <param name="gregorianDate">Gregorian date string</param>
        /// <returns>DateTime</returns>
        public static DateTime? GetDate(string gregorianDate)
        {
            if (string.IsNullOrWhiteSpace(gregorianDate))
                return null;

            try
            {

                return DateTime.ParseExact(gregorianDate,
                    new string[] {
                        "yyyy-MM-dd",
                        "yyyy/MM/dd",
                        "yyyy/M/dd",
                        "yyyy/M/d",
                        "yyyy/MM/d",
                        "yyyy-MM-dd hh:mm:ss",
                        "yyyy/MM/dd hh:mm:ss tt"
                    }
                    ,
                    new CultureInfo("en-US"),
                    DateTimeStyles.None);


            }
            catch (Exception)
            {

                return null;
            }

        }

        public static DateTime? GetDateMelitary(string gregorianDate)
        {
            if (string.IsNullOrWhiteSpace(gregorianDate))
                return null;

            try
            {

                return DateTime.ParseExact(gregorianDate,
                    new string[] {
                        "yyyy-MM-dd",
                        "yyyy/MM/dd",
                        "yyyy/M/dd",
                        "yyyy/M/d",
                        "yyyy/MM/d",
                        "yyyy-MM-dd HH:mm:ss",
                         "yyyy/MM/dd HH:mm:ss"
                    }
                    ,
                    new CultureInfo("en-US"),
                    DateTimeStyles.None);


            }
            catch (Exception)
            {

                return null;
            }

        }


        public static DateTime? GetAVLDate(string gregorianDate)
        {
            if (string.IsNullOrWhiteSpace(gregorianDate))
                return null;
            gregorianDate = gregorianDate.ToLower();

            try
            {

                return DateTime.ParseExact(gregorianDate,
                    new string[] {
                           "M/d/yyyy h:m:s tt"
                    }
                    ,
                    new CultureInfo("en-US"), DateTimeStyles.None);
            }
            catch
            {
                return null;
            }

        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
