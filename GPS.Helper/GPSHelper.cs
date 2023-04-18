using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GPS.Helper
{
    public static class GPSHelper
    {
        public static TimeSpan? ToTimeSpan(string value)
        {
            TimeSpan result = new TimeSpan();
            if (!string.IsNullOrEmpty(value))
            {
                if (!TimeSpan.TryParse(value.ToString(), out result))
                {
                    return null;
                }
            }
            return result;
        }

        public static Int32 ToInt32(Object value)
        {
            Int32 result = 0;
            if (value != null)
            {
                Int32.TryParse(value.ToString(), out result);
            }
            return result;
        }

        public static double ToDouble(string value)
        {
            double result = 0;
            if (value != null)
            {
                double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result);
            }
            return result;
        }

        public static double? ToNullableDouble(string value)
        {
            double result = 0;
            if (value != null)
            {
                if (!double.TryParse(value.ToString(), out result))
                {
                    return null;
                }
            }
            return result;
        }

        public static float? ToFloat(Object value)
        {
            float result = 0;
            if (value != null)
            {
                if (!float.TryParse(value.ToString(), out result))
                {
                    return null;
                }
            }
            return result;
        }

        public static long ToLong(Object value)
        {
            long result = 0;
            if (value != null)
            {
                long.TryParse(value.ToString(), out result);
            }
            return result;

        }

        public static Boolean ToBoolean(Object value)
        {
            Boolean result = false;
            if (value != null)
            {
                Boolean.TryParse(value.ToString(), out result);
            }
            return result;
        }

        public static bool? ToNullableBool(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return value.Equals("1") ? true : false;
            }
            return null;
        }

        public static int? ToNullableInt(Object value)
        {
            int? result = null;
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                if (int.TryParse(value.ToString(), out int val))
                {
                    result = val;
                }
            }
            return result;
        }

        public static long? ToNullableLong(Object value)
        {
            int? result = null;
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                if (int.TryParse(value.ToString(), out int val))
                {
                    result = val;
                }
            }
            return result;
        }

        public static Decimal ToDecimal(Object value)
        {
            Decimal result = 0;
            if (value != null)
            {
                Decimal.TryParse(value.ToString(), out result);
            }
            return result;
        }

        public static String ToBase64String(Object value)
        {
            String result = String.Empty;
            if (value != null)
            {
                result = "data:image/png;base64," + Convert.ToBase64String((Byte[])value);
            }
            return result;
        }

        public static string DateToFormatedString(DateTime date)
        {
            return date.ToString("yyyy/MM/dd", new CultureInfo("en-US").DateTimeFormat);
        }

        public static DateTime StringToDateTime(string date)
        {
            return Convert.ToDateTime(date, new CultureInfo("en-US").DateTimeFormat);
        }
        public static bool IsEmptyDatetime(DateTime? date)
        {
            return !date.HasValue || date == default(DateTime);
        }
        public static string ToDateTimeString(DateTime date, bool withMillisecond = true)
        {
            if (withMillisecond)
            {
                return date.ToString("yyyy/MM/dd hh:mm:ss.fff tt", new CultureInfo("en-US").DateTimeFormat);
            }
            else
            {
                return date.ToString("yyyy/MM/dd hh:mm:ss tt", new CultureInfo("en-US").DateTimeFormat);
            }
        }

        public static string ToGPSDateTime(DateTime date, bool ago = false)
        {
            string result;
            if (date.Date == DateTime.Now.Date)
            {
                result = date.ToShortTimeString();
            }
            else
            {
                result = ToDateTimeString(date, false);
            }
            if (ago)
            {
                result = result + " (" + ToAgo(date) + ")";
            }
            return result;
        }

        private static string ToAgo(DateTime date)
        {
            string result;
            DateTime now = DateTime.Now;
            var diff = now - date;
            int TotalSeconds = (int)diff.TotalSeconds;
            int TotalMinutes = (int)diff.TotalMinutes;
            int TotalHours = (int)diff.TotalHours;
            int TotalDays = (int)diff.TotalDays;

            if (TotalDays == 0 && TotalSeconds < 60)
            {
                result = TotalSeconds + " sec";
            }
            else if (TotalDays == 0 && TotalSeconds > 60 && TotalMinutes < 60)
            {
                result = TotalMinutes + " mins";
            }
            else if (TotalDays < 1 && TotalMinutes >= 60)
            {
                result = TotalHours + " hours";
            }
            else if (TotalDays >= 1 && TotalDays < 31)
            {
                result = TotalDays + " days";
            }
            else if (TotalDays >= 31 && TotalDays < 366)
            {
                result = Math.Abs(TotalDays) / 30 + " month";
            }
            else if (TotalDays >= 366)
            {
                result = Math.Abs(TotalDays) / 365 + " years";
            }
            else
            {
                result = TotalMinutes + " mins";
            }

            return result;
        }

        public static string DatesDifferenceHMS(DateTime from, DateTime to)
        {
            var diff = to - from;

            var secondsS = diff.Seconds.ToString().Length == 1 ? "0" + diff.Seconds.ToString() : diff.Seconds.ToString();
            var minutesS = diff.Minutes.ToString().Length == 1 ? "0" + diff.Minutes.ToString() : diff.Minutes.ToString();
            var hoursS = diff.Hours.ToString().Length == 1 ? "0" + diff.Hours.ToString() : diff.Hours.ToString();

            return hoursS + ":" + minutesS + ":" + secondsS;
        }

        public static string TimeSpanToString(TimeSpan timeSpan)
        {
            var secondsS = timeSpan.Seconds.ToString().Length == 1 ? "0" + timeSpan.Seconds.ToString() : timeSpan.Seconds.ToString();
            var minutesS = timeSpan.Minutes.ToString().Length == 1 ? "0" + timeSpan.Minutes.ToString() : timeSpan.Minutes.ToString();
            var hoursS = timeSpan.Hours.ToString().Length == 1 ? "0" + timeSpan.Hours.ToString() : timeSpan.Hours.ToString();

            if (timeSpan.Days > 0)
            {
                var days = timeSpan.Days;
                var hours = timeSpan.Hours;
                while (days > 0)
                {
                    hours += 24;
                    days--;
                }

                hoursS = hours.ToString().Length == 1 ? "0" + hours.ToString() : hours.ToString();
            }

            return $"{hoursS}h:{minutesS}m:{secondsS}s";
        }


        public static string GetMovementStatus(bool Ignition, int Speed)
        {
            if (Speed > 0 && Ignition)
            {
                return "moving";
            }
            else if (Speed == 0 && Ignition)
            {
                return "p-stopped";
            }
            else if (Speed == 0 && !Ignition)
            {
                return "parked";
            }
            else if (Speed > 0 && !Ignition)
            {
                return "towed";
            }
            return "";
        }

        public static DayOfWeekLookupView GetDayOfWeekLookup(DayOfWeek dayOfWeekEnum)
        {
            switch (dayOfWeekEnum)
            {
                case DayOfWeek.Sunday:
                    return new DayOfWeekLookupView
                    {
                        Name = "الاحد",
                        NameEn = "Sunday",
                    };
                case DayOfWeek.Monday:
                    return new DayOfWeekLookupView
                    {
                        Name = "الاثنين",
                        NameEn = "Monday",
                    };
                case DayOfWeek.Tuesday:
                    return new DayOfWeekLookupView
                    {
                        Name = "الثلاثاء",
                        NameEn = "Tuesday",
                    };
                case DayOfWeek.Wednesday:
                    return new DayOfWeekLookupView
                    {
                        Name = "الاربعاء",
                        NameEn = "Wednesday",
                    };
                case DayOfWeek.Thursday:
                    return new DayOfWeekLookupView
                    {
                        Name = "الخميس",
                        NameEn = "Thursday",
                    };
                case DayOfWeek.Friday:
                    return new DayOfWeekLookupView
                    {
                        Name = "الجمعة",
                        NameEn = "Friday",
                    };
                case DayOfWeek.Saturday:
                    return new DayOfWeekLookupView
                    {
                        Name = "السبت",
                        NameEn = "Saturday",
                    };
                default:
                    return null;
            }
        }

        public static string GetActivityTypeName(string activityType)
        {
            switch (activityType)
            {
                case "DEFAULT":
                    return "تتبع شاحنات";
                case "SFDA":
                    return "SFDA";
                case "AIRPORT_TAXI":
                    return "أجرة المطار";
                case "PUBLIC_TAXI":
                    return "الأجرة العامة";
                case "TOW_CAR":
                    return "سطحة";
                default:
                    return "";
            }
        }

        public static string GetSFDACompanyActivityName(string activity)
        {
            switch (activity)
            {
                case "STORE":
                    return "STORE";
                case "TRANSPORT":
                    return "TRANSPORT";
                case "STORE_TRANSPORT":
                    return "STORE_TRANSPORT";
                default:
                    return "";
            }
        }

        public static string GetSFDAStoringCategoryName(string activity)
        {
            switch (activity)
            {
                case "SCD1": return "SCD1 / Frozen / -20°C to -10°C";
                case "SCD2": return "SCD2 / Chilled / 2°C to 8°C";
                case "SCD3": return "SCD3 / Room Temperature / Less than 25°C";
                case "SCC1": return "SCC1 / Room Temperature / Less than 25°C";
                case "SCM1": return "SCM1 / Frozen / -20°C to -10°C";
                case "SCM2": return "SCM2 / Chilled / 2°C to 8°C";
                case "SCM3": return "SCM3 / Cold Storage / 8°C to 15°C";
                case "SCM4": return "SCM4 / Room Temperature / 15°C to 30°C";
                case "SCM5": return "SCM5 / No Heat Exposure / Should not exceed 40°C";
                case "SCF1": return "SCF1 / Dry / Should not exceed 25°C";
                case "SCF2": return "SCF2 / Chilled / -1.5°C to 10°C";
                case "SCF3": return "SCF3 / Chilled Vegetables and Fruits / -1.5°C to 21°C / Frozen/ Should not exceed (-18)°C";
                case "SCA1": return "SCA1 / Fodder / Should not exceed 30°C";
                case "SCP1": return "SCP1 / Pesticides / Should not exceed 35°C";
                default:
                    return activity;
            }
        }

        public static string GetMovementStatusText(bool Ignition, int Speed, bool isEnglish)
        {
            if (Speed > 0)
            {
                return isEnglish ? "Moving" : "متحركة";
            }
            else
            {
                return isEnglish ? "Stopped" : "متوقفة";
            }
        }


        private static object lockObj = new object();
        public static void LogHistory(string job)
        {
            try
            {
                lock (lockObj)
                {
                    using (StreamWriter writetext = File.AppendText(@"C:\temp\JobHistory.txt"))
                    {
                        writetext.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt", new CultureInfo("en-US").DateTimeFormat)}: {job}");
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        //public static void LogJobError(string error)
        //{
        //    try
        //    {
        //        lock (lockObj)
        //        {
        //            using (StreamWriter writetext = File.AppendText(@"C:\temp\JobError.txt"))
        //            {
        //                writetext.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt", new CultureInfo("en-US").DateTimeFormat)}: {error}");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        private static bool IsEnglishLetter(string letter)
        {
            var EnglishLetters = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            return EnglishLetters.Contains(letter.ToUpper());
        }

        public static List<string> StringToList(string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                return obj.Split(",").ToList();
            }
            return new List<string>();
        }
        public static List<long> StringToListLong(string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                return obj.Split(",").ToList().ConvertAll(x => Int64.Parse(x));
            }
            return new List<long>();
        }
    }
}
