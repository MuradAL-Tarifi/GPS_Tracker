using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.Shared.AppCode.Extensions.Alerts
{
    public static class AlertExtensions
    {
        /// <summary>
        /// Display modal alert with success message
        /// </summary>
        /// <param name="title"></param>
        /// <param name="body"></param>
        public static IActionResult WithSuccess(this IActionResult result, string title, string body, string returnUrl = null)
        {
            return Alert(result, "alert", "success", title, body, returnUrl);
        }

        /// <summary>
        /// Display modal alert with two options and success message<br/>
        /// First option : Redirect to provided returnUrl<br/>
        /// Second option : Close modal and stay in the same View<br/>
        /// </summary>
        /// <param name="result"></param>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="cancelText"></param>
        public static IActionResult WithSuccessOptions(this IActionResult result, string title, string body, string cancelText, string returnUrl)
        {
            return Alert(result, "options", "success", title, body, cancelText, returnUrl);
        }

        /// <summary>
        /// Display modal alert with info message
        /// </summary>
        /// <param name="title"></param>
        /// <param name="body"></param>
        public static IActionResult WithInfo(this IActionResult result, string title, string body)
        {
            return Alert(result, "alert", "info", title, body);
        }

        /// <summary>
        /// Display modal alert with warning message
        /// </summary>
        /// <param name="title"></param>
        /// <param name="body"></param>
        public static IActionResult WithWarning(this IActionResult result, string title, string body)
        {
            return Alert(result, "alert", "warning", title, body);
        }

        /// <summary>
        /// Display modal alert with warning message
        /// First option : Redirect to provided returnUrl<br/>
        /// Second option : Close modal and stay in the same View<br/>
        /// </summary>
        /// <param name="result"></param>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="cancelText"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public static IActionResult WithWarningOptions(this IActionResult result, string title, string body, string cancelText, string returnUrl)
        {
            return Alert(result, "options", "warning", title, body, cancelText, returnUrl);
        }

        /// <summary>
        /// Display modal alert with error message
        /// </summary>
        /// <param name="title"></param>
        /// <param name="body"></param>
        public static IActionResult WithDanger(this IActionResult result, string title, string body)
        {
            return Alert(result, "alert", "error", title, body);
        }

        private static IActionResult Alert(IActionResult result, string alertType, string type, string title, string body, string cancelText = "", string returnUrl = "")
        {
            return new AlertDecoratorResult(result, alertType, type, title, body, cancelText, returnUrl);
        }
    }
}
