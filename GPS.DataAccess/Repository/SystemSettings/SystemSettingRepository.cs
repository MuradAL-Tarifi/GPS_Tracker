using GPS.DataAccess.Context;
using GPS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.SystemSettings
{
    public class SystemSettingRepository : ISystemSettingRepository
    {
        private readonly TrackerDBContext _dbContext;

        public SystemSettingRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddAsync(SystemSetting systemSetting)
        {
            var model = await _dbContext.SystemSetting.FirstOrDefaultAsync();
            if (model != null)
            {
                return await UpdateAsync(systemSetting);
            }
            systemSetting.CreatedDate = DateTime.Now;
            if (systemSetting.LogoPhotoByte.Count() == 0)
            {
                systemSetting.LogoPhotoByte = null;
            }

            await _dbContext.SystemSetting.AddAsync(systemSetting);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<SystemSetting> LoadSystemSettingAsync()
        {
            return await _dbContext.SystemSetting.FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(SystemSetting systemSetting)
        {
            var model = await _dbContext.SystemSetting.FindAsync(systemSetting.Id);
            if (model == null)
            {
                return false;
            }

            if (systemSetting.LogoPhotoByte.Length > 0)
            {
                model.LogoPhotoByte = systemSetting.LogoPhotoByte;
            }

            model.GoogleApiKey = systemSetting.GoogleApiKey;
            model.WaslApiKey = systemSetting.WaslApiKey;
            model.CompanyName = systemSetting.CompanyName;

            model.SMTP_Address = systemSetting.SMTP_Address;
            model.SMTP_HOST = systemSetting.SMTP_HOST;
            model.SMTP_PORT = systemSetting.SMTP_PORT;
            model.SMTP_Password = systemSetting.SMTP_Password;
            model.SMTP_DisplayName = systemSetting.SMTP_DisplayName;
            model.SMTP_IsSslEnabled = systemSetting.SMTP_IsSslEnabled;
            model.EnableSMTP = systemSetting.EnableSMTP;

            model.SMS_GatewayURL = systemSetting.SMS_GatewayURL;
            model.SMS_Username = systemSetting.SMS_Username;
            model.SMS_Password = systemSetting.SMS_Password;
            model.EnableSMS = systemSetting.EnableSMS;

            model.UpdatedBy = systemSetting.UpdatedBy;
            model.UpdatedDate = DateTime.Now;

            _dbContext.SystemSetting.Update(model);
            await _dbContext.SaveChangesAsync();
            return true;
        }

    }
}
