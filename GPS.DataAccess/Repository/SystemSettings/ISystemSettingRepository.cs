using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPS.Domain.Models;

namespace GPS.DataAccess.Repository.SystemSettings
{
   public interface ISystemSettingRepository
    {
        Task<bool> AddAsync(SystemSetting systemSetting);
        Task<bool> UpdateAsync(SystemSetting systemSetting);
        Task<SystemSetting> LoadSystemSettingAsync();
    }
}
