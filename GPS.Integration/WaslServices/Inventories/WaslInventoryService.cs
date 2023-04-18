using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Integration.WaslIntegrations.Inventories;
using GPS.Integration.WaslModels;
using GPS.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Integration.WaslServices.Inventories
{
    public class WaslInventoryService : IWaslInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WaslInventoryService> _logger;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        private readonly IWaslIntegrationInventory _waslInventory;

        public WaslInventoryService(
            IUnitOfWork unitOfWork,
            ILogger<WaslInventoryService> logger,
            IStringLocalizer<SharedResources> sharedLocalizer,
            IWaslIntegrationInventory waslInventory)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
            _waslInventory = waslInventory;
        }

        public async Task<ReturnResult<WaslResult>> RegisterAsync(string companyId, string warehouseId, WaslInventoryModel model)
        {
            var result = new ReturnResult<WaslResult>();

            try
            {
                var waslResponse = await _waslInventory.RegisterAsync(companyId, warehouseId, model);
                if (waslResponse != null)
                {
                    if (waslResponse.Success)
                    {
                        result.Success(waslResponse.Result);
                    }
                    else
                    {
                        var errorMessageList = new List<string>();
                        var resultCode = await _unitOfWork.LookupsRepository.GetWaslResultCode(waslResponse.ResultCode);
                        if (resultCode != null)
                        {
                            errorMessageList.Add(resultCode.MessageAr);
                            errorMessageList.Add(resultCode.MessageEn);
                            result.BadRequest(errorMessageList);
                            result.Data = waslResponse.Result;
                        }
                        else
                        {
                            result.ServerError(_sharedLocalizer["IntegrationError"]);
                        }
                    }
                }
                else
                {
                    result.ServerError(_sharedLocalizer["IntegrationError"]);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(_sharedLocalizer["SystemError"]);
            }

            return result;
        }

        public async Task<ReturnResult<WaslResult>> UpdateAsync(string inventoryId, WaslInventoryUpdateModel model)
        {
            var result = new ReturnResult<WaslResult>();

            try
            {
                var waslResponse = await _waslInventory.UpdateAsync(inventoryId, model);
                if (waslResponse != null)
                {
                    if (waslResponse.Success)
                    {
                        result.Success(waslResponse.Result);
                    }
                    else
                    {
                        var errorMessageList = new List<string>();
                        var resultCode = await _unitOfWork.LookupsRepository.GetWaslResultCode(waslResponse.ResultCode);
                        if (resultCode != null)
                        {
                            errorMessageList.Add(resultCode.MessageAr);
                            errorMessageList.Add(resultCode.MessageEn);
                            result.BadRequest(errorMessageList);
                            result.Data = waslResponse.Result;
                        }
                        else
                        {
                            result.ServerError(_sharedLocalizer["IntegrationError"]);
                        }
                    }
                }
                else
                {
                    result.ServerError(_sharedLocalizer["IntegrationError"]);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(_sharedLocalizer["SystemError"]);
            }

            return result;
        }

        public async Task<ReturnResult<WaslResult>> DeleteAsync(string inventoryId)
        {
            var result = new ReturnResult<WaslResult>();

            try
            {
                var waslResponse = await _waslInventory.DeleteAsync(inventoryId);
                if (waslResponse != null)
                {
                    if (waslResponse.Success)
                    {
                        result.Success(waslResponse.Result);
                    }
                    else
                    {
                        var errorMessageList = new List<string>();
                        var resultCode = await _unitOfWork.LookupsRepository.GetWaslResultCode(waslResponse.ResultCode);
                        if (resultCode != null)
                        {
                            errorMessageList.Add(resultCode.MessageAr);
                            errorMessageList.Add(resultCode.MessageEn);
                            result.BadRequest(errorMessageList);
                            result.Data = waslResponse.Result;
                        }
                        else
                        {
                            result.ServerError(_sharedLocalizer["IntegrationError"]);
                        }
                    }
                }
                else
                {
                    result.ServerError(_sharedLocalizer["IntegrationError"]);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(_sharedLocalizer["SystemError"]);
            }

            return result;
        }
    }
}
