using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Models.Enums
{
    public enum SensorAlertTypeLookupEnum
    {
        TempretureRange = 1
    }
    public enum HttpCode
    {
        /// <summary>
        /// تمت العملية بنجاح
        /// </summary>
        Success = 200,
        /// <summary>
        /// خطأ في القيم الممررة
        /// </summary>
        BadRequest = 400,
        /// <summary>
        /// دخول غير مصرح
        /// </summary>
        Unauthorized = 401,
        /// <summary>
        /// البيانات غير موجودة
        /// </summary>
        NotFound = 404,
        /// <summary>
        /// حدث خطأ في النظام
        /// </summary>
        ServerError = 500
    }
}
