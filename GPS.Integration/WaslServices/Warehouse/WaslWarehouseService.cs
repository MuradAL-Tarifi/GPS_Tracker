using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Integration.WaslIntegrations.Warehouse;
using GPS.Integration.WaslModels;
using GPS.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Integration.WaslServices.Warehouse
{
    public class WaslWarehouseService : IWaslWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WaslWarehouseService> _logger;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        private readonly IWaslIntegrationWarehouse _waslWarehouse;

        public WaslWarehouseService(
            IUnitOfWork unitOfWork,
            ILogger<WaslWarehouseService> logger,
            IStringLocalizer<SharedResources> sharedLocalizer,
            IWaslIntegrationWarehouse waslWarehouse)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
            _waslWarehouse = waslWarehouse;
        }

        public async Task<ReturnResult<WaslResult>> RegisterAsync(string companyId, WaslWarehouseModel model)
        {
            var result = new ReturnResult<WaslResult>();

            try
            {
                var waslResponse = await _waslWarehouse.RegisterAsync(companyId, model);
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

        public async Task<ReturnResult<WaslResult>> UpdateAsync(string warehouseId, WaslWarehouseUpdateModel model)
        {
            var result = new ReturnResult<WaslResult>();

            try
            {
                var waslResponse = await _waslWarehouse.UpdateAsync(warehouseId, model);
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

        public async Task<ReturnResult<WaslResult>> DeleteAsync(string warehouseId)
        {
            var result = new ReturnResult<WaslResult>();

            try
            {
                var waslResponse = await _waslWarehouse.DeleteAsync(warehouseId);
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


        public async Task<ReturnResult<WaslInquiryModel>> InquiryAsync(string companyId)
        {
            var result = new ReturnResult<WaslInquiryModel>();

            try
            {
                if (string.IsNullOrWhiteSpace(companyId))
                {
                    result.BadRequest(_sharedLocalizer["RequiredCompanyId"]);
                    return result;
                }

                var warehouse = await _waslWarehouse.InquiryAsync(companyId);
                if (warehouse != null)
                {
                    result.Success(warehouse);
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
