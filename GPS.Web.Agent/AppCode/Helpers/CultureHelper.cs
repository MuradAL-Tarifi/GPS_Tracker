using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GPS.Web.Agent.AppCode.Helpers
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
        public string GetLocalizedName(string Arabic, string English)
        {
            string language = Thread.CurrentThread.CurrentCulture.Name;
            switch (language)
            {
                case "en-US":
                    return English;
                case "ar-SA":
                    return Arabic;
                default:
                    return Arabic;
            }
        }
    }
}
