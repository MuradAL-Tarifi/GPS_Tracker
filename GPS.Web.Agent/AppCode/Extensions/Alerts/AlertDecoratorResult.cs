using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace GPS.Web.Agent.AppCode.Extensions.Alerts
{
    public class AlertDecoratorResult : IActionResult
    {
        public IActionResult Result { get; }
        public string AlertType { get; }
        public string Type { get; }
        public string Title { get; }
        public string Text { get; }
        public string CancelText { get; }
        public string ReturnUrl { get; }

        public AlertDecoratorResult(IActionResult result, string alertType, string type, string title, string text, string cancelText, string returnUrl)
        {
            Result = result;
            AlertType = alertType;
            Type = type;
            Title = title;
            Text = text;
            CancelText = cancelText;
            ReturnUrl = returnUrl;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            //NOTE: Be sure you add a using statement for Microsoft.Extensions.DependencyInjection, otherwise
            //      this overload of GetService won't be available!
            var factory = context.HttpContext.RequestServices.GetService<ITempDataDictionaryFactory>();

            var tempData = factory.GetTempData(context.HttpContext);
            tempData["_alert.alertType"] = AlertType;
            tempData["_alert.type"] = Type;
            tempData["_alert.title"] = Title;
            tempData["_alert.body"] = Text;
            tempData["_alert.cancelText"] = CancelText;
            tempData["_alert.returnUrl"] = ReturnUrl;

            await Result.ExecuteResultAsync(context);
        }
    }
}
