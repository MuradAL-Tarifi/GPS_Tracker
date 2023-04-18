using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GPS.Shared.AppCode.Helpers
{
    public interface ICultureHelper
    {
        /// <summary>
        /// Get the Name based on current culture
        /// </summary>
        /// <param name="arabic"></param>
        /// <param name="english"></param>
        string GetLocalizedName(string arabic, string english);
    }

    public class CultureHelper : ICultureHelper
    {
        public string GetLocalizedName(string arabic, string english)
        {
            string language = Thread.CurrentThread.CurrentCulture.Name;
            switch (language)
            {
                case "en-US":
                    return english;
                case "ar-SA":
                    return arabic;
                default:
                    return arabic;
            }
        }
    }
}
