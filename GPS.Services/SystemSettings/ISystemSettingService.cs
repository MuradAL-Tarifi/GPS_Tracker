using GPS.Domain.DTO;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.SystemSettings
{
   public interface ISystemSettingService
    {
        /// <summary>
        /// SaveAsync SystemSetting
        /// </summary>
        /// <param name="systemSetting"></param>
        /// <returns></returns>
        Task<ReturnResult<bool>> SaveAsync(SystemSettingView systemSetting);
        /// <summary>
        /// Load System Setting
        /// </summary>
        /// <returns></returns>
        Task<ReturnResult<SystemSettingView>> LoadSystemSettingAsync();
    }
}
