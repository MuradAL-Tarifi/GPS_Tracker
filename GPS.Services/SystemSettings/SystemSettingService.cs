using AutoMapper;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using GPS.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.SystemSettings
{
   public class SystemSettingService : ISystemSettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SystemSettingService> _logger;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public SystemSettingService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SystemSettingService> logger,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
        }

        public async Task<ReturnResult<bool>> SaveAsync(SystemSettingView systemSetting)
        {
            if(systemSetting.Id > 0)
            {
                return await UpdateAsync(systemSetting);
            }
            else
            {
                return await AddAsync(systemSetting);
            }
        }

        private async Task<ReturnResult<bool>> UpdateAsync(SystemSettingView systemSetting)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var updated = await _unitOfWork.SystemSettingRepository.UpdateAsync(_mapper.Map<SystemSetting>(systemSetting));
                systemSetting.LogoPhotoByte = null;
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, systemSetting.Id, systemSetting, systemSetting.UpdatedBy);

                result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
                result.Data = false;
            }
            return result;
        }

        private async Task<ReturnResult<bool>> AddAsync(SystemSettingView systemSetting)
        {
            var result = new ReturnResult<bool>();
            try
            {
                await _unitOfWork.SystemSettingRepository.AddAsync(_mapper.Map<SystemSetting>(systemSetting));
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.create, systemSetting.Id, systemSetting, systemSetting.CreatedBy);
                result.Success(true);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
                result.Data = false;
            }
            return result;
        }

        public async Task<ReturnResult<SystemSettingView>> LoadSystemSettingAsync()
        {
            var result = new ReturnResult<SystemSettingView>();
            try
            {
                var systemSetting = _mapper.Map<SystemSettingView>(await _unitOfWork.SystemSettingRepository.LoadSystemSettingAsync());
                if(systemSetting != null)
                {
                    if (systemSetting.LogoPhotoByte != null)
                    {
                        systemSetting.LogoFileBase64 = Convert.ToBase64String(systemSetting.LogoPhotoByte);
                    }
                    result.Success(systemSetting);
                }
                else
                {
                    result.Success(new SystemSettingView());
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }
    }
}
