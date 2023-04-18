using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Integration.WaslIntegrations.OperatingCompanies;
using GPS.Integration.WaslModels;
using GPS.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Integration.WaslServices.OperatingCompanies
{
    public class WaslOperatingCompaniesService : IWaslOperatingCompaniesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WaslOperatingCompaniesService> _logger;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        private readonly IWaslIntegrationOperatingCompanies _waslOperatingCompanies;

        public WaslOperatingCompaniesService(
            IUnitOfWork unitOfWork,
           ILogger<WaslOperatingCompaniesService> logger,
            IStringLocalizer<SharedResources> sharedLocalizer,
            IWaslIntegrationOperatingCompanies waslOperatingCompanies)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
            _waslOperatingCompanies = waslOperatingCompanies;
        }

        public async Task<ReturnResult<WaslCompany>> GetAsync(string identityNumber, string commercialRecordNumber, string activity)
        {
            var result = new ReturnResult<WaslCompany>();

            try
            {
                var errorList = new List<string>();
                if (string.IsNullOrWhiteSpace(identityNumber))
                {
                    errorList.Add(_sharedLocalizer["RequiredIdentityNumber"]);
                }

                if (string.IsNullOrWhiteSpace(commercialRecordNumber))
                {
                    errorList.Add(_sharedLocalizer["RequiredCommercialRecordNumber"]);
                }

                if (errorList.Count > 0)
                {
                    result.BadRequest(errorList);
                    return result;
                }

                var company = await _waslOperatingCompanies.GetCompanyAsync(identityNumber, commercialRecordNumber, activity);
                if (company != null)
                {
                    result.Success(company);
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

        public async Task<ReturnResult<WaslInquiryModel>> InquiryAsync(string identityNumber, string commercialRecordNumber, string activity)
        {
            var result = new ReturnResult<WaslInquiryModel>();

            try
            {
                var errorList = new List<string>();
                if (string.IsNullOrWhiteSpace(identityNumber))
                {
                    errorList.Add(_sharedLocalizer["RequiredIdentityNumber"]);
                }

                if (string.IsNullOrWhiteSpace(commercialRecordNumber))
                {
                    errorList.Add(_sharedLocalizer["RequiredCommercialRecordNumber"]);
                }

                if (errorList.Count > 0)
                {
                    result.BadRequest(errorList);
                    return result;
                }

                if (!string.IsNullOrEmpty(commercialRecordNumber))
                {
                    var company = await _waslOperatingCompanies.CompanyInquiryAsync(identityNumber, commercialRecordNumber, activity);
                    if (company != null)
                    {
                        result.Success(company);
                    }
                    else
                    {
                        result.ServerError(_sharedLocalizer["IntegrationError"]);
                    }
                }
                else
                {
                    var Iindividual = await _waslOperatingCompanies.IndividualInquiryAsync(identityNumber, activity);
                    if (Iindividual != null)
                    {
                        result.Success(Iindividual);
                    }
                    else
                    {
                        result.ServerError(_sharedLocalizer["IntegrationError"]);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(_sharedLocalizer["SystemError"]);
            }

            return result;
        }

        public async Task<ReturnResult<WaslResult>> RegisterCompanyAsync(WaslRegisterCompanyModel model)
        {
            var result = new ReturnResult<WaslResult>();

            try
            {
                var waslResponse = await _waslOperatingCompanies.RegisterCompanyAsync(model);
                if (waslResponse != null)
                {
                    if (waslResponse.Success)
                    {
                        result.Success(waslResponse.Result);
                    }
                    else
                    {
                        if (waslResponse.Result == null)
                        {
                            // get reference key from inquiry service
                            var waslCompany = await _waslOperatingCompanies.GetCompanyAsync(model.IdentityNumber, model.CommercialRecordNumber, model.Activity);
                            if (waslCompany != null && !string.IsNullOrEmpty(waslCompany.ReferenceNumber))
                            {
                                waslResponse.Result = new WaslResult()
                                {
                                    ReferenceKey = waslCompany.ReferenceNumber,
                                    IsDuplicate = true
                                };

                                result.Data = waslResponse.Result;
                            }
                            else
                            {
                                result.ServerError($"{_sharedLocalizer["IntegrationError"]} - {waslResponse.ResultCode}");
                            }
                        }
                        else
                        {
                            waslResponse.Result.IsDuplicate = waslResponse.ResultCode.Contains("duplicate");
                            var errorMessageList = new List<string>();
                            var resultCode = await _unitOfWork.LookupsRepository.GetWaslResultCode(waslResponse.ResultCode);
                            errorMessageList.Add(resultCode.MessageAr);
                            errorMessageList.Add(resultCode.MessageEn);
                            result.BadRequest(errorMessageList);
                            result.Data = waslResponse.Result;
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

        public async Task<ReturnResult<WaslResult>> RegisterIndividualAsync(WaslIndividualModel model)
        {
            var result = new ReturnResult<WaslResult>();

            try
            {
                var waslResponse = await _waslOperatingCompanies.RegisterIndividualAsync(model);
                if (waslResponse != null)
                {

                    if (waslResponse.Success)
                    {
                        result.Success(waslResponse.Result);
                    }
                    else
                    {
                        if (waslResponse.Result == null)
                        {
                            // get reference key from inquiry service
                            var waslIndividual = await _waslOperatingCompanies.GetIndividualAsync(model.IdentityNumber, model.Activity);
                            if (waslIndividual != null)
                            {
                                waslResponse.Result = new WaslResult()
                                {
                                    ReferenceKey = waslIndividual.ReferenceNumber,
                                    IsDuplicate = true
                                };

                                result.Data = waslResponse.Result;
                            }
                            else
                            {
                                result.ServerError($"{_sharedLocalizer["IntegrationError"]} - {waslResponse.ResultCode}");
                            }
                        }
                        else
                        {
                            waslResponse.Result.IsDuplicate = waslResponse.ResultCode.Contains("duplicate");
                            var errorMessageList = new List<string>();
                            var resultCode = await _unitOfWork.LookupsRepository.GetWaslResultCode(waslResponse.ResultCode);
                            errorMessageList.Add(resultCode.MessageAr);
                            errorMessageList.Add(resultCode.MessageEn);
                            result.BadRequest(errorMessageList);
                            result.Data = waslResponse.Result;
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

        public async Task<ReturnResult<WaslResult>> UpdateContactInfoAsync(WaslUpdateCompanyModel model)
        {
            var result = new ReturnResult<WaslResult>();

            try
            {
                var waslResponse = await _waslOperatingCompanies.UpdateContactInfoAsync(model);
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
                        errorMessageList.Add(resultCode.MessageAr);
                        errorMessageList.Add(resultCode.MessageEn);
                        result.BadRequest(errorMessageList);
                        result.Data = waslResponse.Result;
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

        public async Task<ReturnResult<WaslResult>> DeleteAsync(string referenceNumber, string activity)
        {
            var result = new ReturnResult<WaslResult>();

            try
            {
                var waslResponse = await _waslOperatingCompanies.DeleteAsync(referenceNumber, activity);
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
                        errorMessageList.Add(resultCode.MessageAr);
                        errorMessageList.Add(resultCode.MessageEn);
                        result.BadRequest(errorMessageList);
                        result.Data = waslResponse.Result;
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
