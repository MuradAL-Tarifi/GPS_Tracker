using AutoMapper;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Integration.WaslModels;
using GPS.Integration.WaslServices.OperatingCompanies;
using GPS.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Fleets
{
    public class FleetService : IFleetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<FleetService> _logger;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        private readonly IWaslOperatingCompaniesService _waslOperatingCompaniesManager;

        public FleetService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<FleetService> logger,
            IStringLocalizer<SharedResources> sharedLocalizer,
            IWaslOperatingCompaniesService waslOperatingCompaniesManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
            _waslOperatingCompaniesManager = waslOperatingCompaniesManager;
        }

        public async Task<ReturnResult<PagedResult<FleetView>>> SearchAsync(string SearchString = "", int? waslLinkStatus = null, int PageNumber = 1, int pageSize = 100)
        {
            var result = new ReturnResult<PagedResult<FleetView>>();
            try
            {
                var pagedResult = await _unitOfWork.FleetRepository.SearchAsync(SearchString, waslLinkStatus, PageNumber, pageSize);

                var pagedListView = new PagedResult<FleetView>
                {
                    TotalRecords = pagedResult.TotalRecords,
                    List = _mapper.Map<List<FleetView>>(pagedResult.List)
                };

                foreach (var item in pagedListView.List)
                {
                    var fleetDetails = await _unitOfWork.FleetRepository.FindDetailsByFleetIdAsync(item.Id);
                    item.FleetDetails = _mapper.Map<FleetDetailsView>(fleetDetails);
                }

                result.Success(pagedListView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<FleetView>> FindByIdAsync(long? Id)
        {
            var result = new ReturnResult<FleetView>();
            try
            {
                var fleet = await _unitOfWork.FleetRepository.FindByIdAsync(Id);
                if (fleet == null)
                {
                    result.NotFound(_sharedLocalizer["FleetNotExists"]);
                    return result;
                }

                var fleetView = _mapper.Map<FleetView>(fleet);
                var fleetDetails = await _unitOfWork.FleetRepository.FindDetailsByFleetIdAsync(Id);
                fleetView.FleetDetails = _mapper.Map<FleetDetailsView>(fleetDetails);

                result.Success(fleetView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<FleetDetailsView>> FindDetailsByIdAsync(long? Id)
        {
            var result = new ReturnResult<FleetDetailsView>();
            try
            {
                var fleet = await _unitOfWork.FleetRepository.FindDetailsByFleetIdAsync(Id);
                if (fleet == null)
                {
                    result.NotFound(_sharedLocalizer["FleetNotExists"]);
                    return result;
                }
                result.Success(_mapper.Map<FleetDetails, FleetDetailsView>(fleet));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<bool>> AddAsync(FleetView fleet)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var newfleet = _mapper.Map<Fleet>(fleet);
                newfleet.CreatedBy = fleet.CreatedBy;
                newfleet.CreatedDate = DateTime.Now;

                var addedEntity= await _unitOfWork.FleetRepository.AddAsync(newfleet);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.create, addedEntity.Id, newfleet, fleet.CreatedBy);
                await _unitOfWork.FleetRepository.AddDefualtFleetDetailsAsync(new FleetDetails() { FleetId = addedEntity.Id });

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

        public async Task<ReturnResult<bool>> UpdateAsync(FleetView model)
        {
            var result = new ReturnResult<bool>();
            try
            {
                await _unitOfWork.FleetRepository.UpdateAsync(model);

                await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, model.Id, model, model.UpdatedBy);

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

        public async Task<ReturnResult<bool>> DeleteAsync(long Id, string UpdatedBy)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var entity = await _unitOfWork.FleetRepository.DeleteAsync(Id, UpdatedBy);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.delete, entity.Id, entity, UpdatedBy);
                await _unitOfWork.FleetRepository.DeleteFleetDetailsAsync(Id);

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

        public async Task<ReturnResult<bool>> LinkWithWasl(long id, string updatedBy)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var fleetDetails = await _unitOfWork.FleetRepository.FindDetailsByFleetIdAsync(id);
                if (fleetDetails == null)
                {
                    result.NotFound(_sharedLocalizer["FleetNotExists"]);
                    return result;
                }

                // التحقق من توفر بيانات الارتباط
                List<string> missingData = new List<string>();
                if (string.IsNullOrEmpty(fleetDetails.IdentityNumber))
                {
                    missingData.Add("رقم الهوية");
                }

                if (string.IsNullOrEmpty(fleetDetails.PhoneNumber))
                {
                    missingData.Add("رقم الهاتف");
                }

                if (string.IsNullOrEmpty(fleetDetails.EmailAddress))
                {
                    missingData.Add("البريد الالكتروني");
                }

                if (fleetDetails.FleetType == "individual")
                {
                    //Individual
                    if (string.IsNullOrEmpty(fleetDetails.DateOfBirthHijri))
                    {
                        missingData.Add("تاريخ الميلاد");
                    }
                }
                else
                {
                    //Company
                    if (string.IsNullOrEmpty(fleetDetails.CommercialRecordNumber))
                    {
                        missingData.Add("رقم السجل التجاري");
                    }

                    if (string.IsNullOrEmpty(fleetDetails.CommercialRecordIssueDateHijri))
                    {
                        missingData.Add("تاريخ السجل التجاري هجري");
                    }

                    if (string.IsNullOrEmpty(fleetDetails.ManagerName))
                    {
                        missingData.Add("اسم المدير");
                    }

                    if (string.IsNullOrEmpty(fleetDetails.ManagerPhoneNumber))
                    {
                        missingData.Add("رقم هاتف المدير");
                    }

                    if (string.IsNullOrEmpty(fleetDetails.ManagerMobileNumber))
                    {
                        missingData.Add("رقم جوال المدير");
                    }
                }


                if (missingData.Count > 0)
                {
                    result.BadRequest($"بيانات الربط غير كاملة. يرجى التحقق من [{string.Join(", ", missingData)}]");
                    result.Data = false;
                    return result;
                }


                var registerResult = new ReturnResult<WaslResult>();
                if (fleetDetails.FleetType == "individual")
                {
                    var waslIndividualModel = new WaslIndividualModel()
                    {
                        IdentityNumber = fleetDetails.IdentityNumber,
                        DateOfBirthHijri = fleetDetails.DateOfBirthHijri,
                        PhoneNumber = fleetDetails.PhoneNumber,
                        ExtensionNumber = fleetDetails.ExtensionNumber,
                        EmailAddress = fleetDetails.EmailAddress,
                        Activity = fleetDetails.ActivityType,
                        SFDACompanyActivity = fleetDetails.SFDACompanyActivities
                    };

                    registerResult = await _waslOperatingCompaniesManager.RegisterIndividualAsync(waslIndividualModel);
                    await _unitOfWork.EventLogRepository.LogEventAsync(Event.registerWaslIndividual, fleetDetails.Id, registerResult, updatedBy);
                }
                else
                {
                    var waslRegisterCompanyModel = new WaslRegisterCompanyModel()
                    {
                        IdentityNumber = fleetDetails.IdentityNumber,
                        CommercialRecordNumber = fleetDetails.CommercialRecordNumber,
                        CommercialRecordIssueDateHijri = fleetDetails.CommercialRecordIssueDateHijri,
                        PhoneNumber = fleetDetails.PhoneNumber,
                        ExtensionNumber = fleetDetails.ExtensionNumber,
                        EmailAddress = fleetDetails.EmailAddress,
                        ManagerName = fleetDetails.ManagerName,
                        ManagerPhoneNumber = fleetDetails.ManagerPhoneNumber,
                        ManagerMobileNumber = fleetDetails.ManagerMobileNumber,
                        Activity = fleetDetails.ActivityType,
                        SFDACompanyActivity = fleetDetails.SFDACompanyActivities
                    };
                    registerResult = await _waslOperatingCompaniesManager.RegisterCompanyAsync(waslRegisterCompanyModel);
                    await _unitOfWork.EventLogRepository.LogEventAsync(Event.registerWaslCompany, fleetDetails.Id, registerResult, updatedBy);
                }

                if ((registerResult.IsSuccess || registerResult.Data?.IsDuplicate == true) && !string.IsNullOrEmpty(registerResult.Data.ReferenceKey))
                {
                    fleetDetails.IsLinkedWithWasl = true;
                    fleetDetails.UpdatedBy = updatedBy;
                    fleetDetails.UpdatedDate = DateTime.Now;
                    fleetDetails.ReferanceNumber = registerResult.Data.ReferenceKey;

                    await _unitOfWork.FleetRepository.UpdateLinkedWithWaslInfoAsync(fleetDetails);

                    await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, fleetDetails.Id, fleetDetails, updatedBy);

                    result.Success(true);
                }
                else
                {
                    result.BadRequest(registerResult.ErrorList);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<bool>> UnlinkWithWasl(long id, string updatedBy)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var fleetDetails = await _unitOfWork.FleetRepository.FindDetailsByFleetIdAsync(id);
                if (fleetDetails == null)
                {
                    result.NotFound(_sharedLocalizer["FleetNotExists"]);
                    return result;
                }

                var isAnyLinkedWithWasl = await _unitOfWork.FleetRepository.IsAnyLinkedWithWaslAsync(id);
                if (isAnyLinkedWithWasl)
                {
                    result.BadRequest("لم يتم فك الربط بسبب وجود مستودع تابع للشركة مرتبط في وصل");
                    result.Data = true; // IsAnyLinkedWithWasl
                    return result;
                }

                // التحقق من توفر بيانات الارتباط
                if (!fleetDetails.IsLinkedWithWasl || string.IsNullOrEmpty(fleetDetails.ReferanceNumber))
                {
                    result.BadRequest("بيانات فط الربط غير كاملة. يرجى التحقق من الرقم المرجعي للشركة");
                    return result;
                }

                var registerResult = await _waslOperatingCompaniesManager.DeleteAsync(fleetDetails.ReferanceNumber, fleetDetails.ActivityType);

                if (string.IsNullOrEmpty(fleetDetails.CommercialRecordNumber))
                {
                    await _unitOfWork.EventLogRepository.LogEventAsync(Event.deleteWaslIndividual, fleetDetails.Id, registerResult, updatedBy);
                }
                else
                {
                    await _unitOfWork.EventLogRepository.LogEventAsync(Event.deleteWaslCompany, fleetDetails.Id, registerResult, updatedBy);
                }

                if (registerResult.IsSuccess)
                {
                    fleetDetails.IsLinkedWithWasl = false;
                    fleetDetails.UpdatedBy = updatedBy;
                    fleetDetails.UpdatedDate = DateTime.Now;

                    await _unitOfWork.FleetRepository.UpdateLinkedWithWaslInfoAsync(fleetDetails);

                    await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, fleetDetails.Id, fleetDetails, updatedBy);

                    result.Success(true);
                }
                else
                {
                    result.BadRequest(registerResult.ErrorList);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<int>> CountAsync()
        {
            var result = new ReturnResult<int>();
            try
            {
                int Count = await _unitOfWork.FleetRepository.CountAsync();
                result.Success(Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<int>> CountLinkedWithWaslAsync()
        {
            var result = new ReturnResult<int>();
            try
            {
                int Count = await _unitOfWork.FleetRepository.CountLinkedWithWaslAsync();
                result.Success(Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<FleetView>>> GetAllAsync(int? AgentId = null)
        {
            var result = new ReturnResult<List<FleetView>>();
            try
            {
                var fleets = await _unitOfWork.FleetRepository.GetAllAsync();
                result.Success(_mapper.Map<List<Fleet>, List<FleetView>>(fleets));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }
        public async Task<ReturnResult<List<FleetView>>> GetFleetsWASLAsync(int? AgentId = null)
        {
            var result = new ReturnResult<List<FleetView>>();
            try
            {
                var fleetsWASL = await _unitOfWork.FleetRepository.GetFleetsWASLAsync(AgentId);

                result.Success(_mapper.Map<List<Fleet>, List<FleetView>>(fleetsWASL));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<bool> IsNameExistsAsync(int AgentId, string Name)
        {
            try
            {
                var exists = await _unitOfWork.FleetRepository.IsNameExistsAsync(AgentId, Name);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return false;
        }

        public async Task<bool> IsNameEnExistsAsync(int AgentId, string NameEn)
        {
            try
            {
                var exists = await _unitOfWork.FleetRepository.IsNameEnExistsAsync(AgentId, NameEn);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return false;
        }

        public async Task<ReturnResult<FleetWaslModel>> GetWaslDetailsAsync(long? fleetId)
        {
            var result = new ReturnResult<FleetWaslModel>();
            try
            {
                var fleet = await _unitOfWork.FleetRepository.FindByIdAsync(fleetId);

                if (fleet == null)
                {
                    result.NotFound(_sharedLocalizer["FleetNotExists"]);
                    return result;
                }

                var fleetDetails = await _unitOfWork.FleetRepository.FindDetailsByFleetIdAsync(fleetId);

                var fleetWaslModel = _mapper.Map<FleetWaslModel>(fleetDetails);
                if (fleet != null && fleetWaslModel == null)
                {
                    fleetWaslModel = new FleetWaslModel();
                    fleetWaslModel.FleetName = fleet.Name;
                    fleetWaslModel.FleetNameEn = fleet.NameEn;
                }
                if (fleetWaslModel != null)
                {
                    fleetWaslModel.FleetId = fleet.Id;
                    fleetWaslModel.FleetName = fleet.Name;
                    fleetWaslModel.FleetNameEn = fleet.NameEn;
                    fleetWaslModel.FleetManagerEmail = fleet.ManagerEmail;
                    fleetWaslModel.FleetManagerMobile = fleet.ManagerMobile;
                    fleetWaslModel.FleetSupervisorEmail = fleet.SupervisorEmail;
                    fleetWaslModel.FleetSupervisorMobile = fleet.SupervisorMobile;
                }
                result.Success(fleetWaslModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<bool>> UpdateWaslDetailsAsync(FleetWaslModel model)
        {
            var result = new ReturnResult<bool>();
            try
            {
                await _unitOfWork.FleetRepository.UpdateWaslDetailsAsync(model);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, model.FleetId, model, model.UpdatedBy);

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

        public async Task<ReturnResult<bool>> AgentUpdateWaslDetailsAsync(FleetWaslModel model)
        {
            var result = new ReturnResult<bool>();
            try
            {
                await _unitOfWork.FleetRepository.AgentUpdateWaslDetailsAsync(model);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, model.FleetId, model, model.UpdatedBy);

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

        public async Task<ReturnResult<bool>> UpdateWaslContactInfoAsync(FleetWaslModel model)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var fleetDetails = await _unitOfWork.FleetRepository.FindDetailsByFleetIdAsync(model.FleetId);
                if (fleetDetails == null)
                {
                    result.NotFound(_sharedLocalizer["FleetNotExists"]);
                    return result;
                }

                if (fleetDetails.FleetType == "individual")
                {
                    result.BadRequest($"تحديث معلومات الاتصال متاح فقط للشركة");
                    result.Data = false;
                    return result;
                }

                if (fleetDetails.ManagerPhoneNumber == model.ManagerPhoneNumber &&
                    fleetDetails.ManagerMobileNumber == model.ManagerMobileNumber &&
                    fleetDetails.ManagerName == model.ManagerName)
                {
                    result.BadRequest($"لم يتم تعديل اي من معلومات الاتصال");
                    result.Data = false;
                    return result;
                }

                // التحقق من توفر بيانات الارتباط
                List<string> missingData = new List<string>();
                if (string.IsNullOrEmpty(fleetDetails.IdentityNumber))
                {
                    missingData.Add("رقم الهوية");
                }

                if (string.IsNullOrEmpty(fleetDetails.CommercialRecordNumber))
                {
                    missingData.Add("رقم السجل التجاري");
                }

                if (string.IsNullOrEmpty(model.ManagerName))
                {
                    missingData.Add("اسم المدير");
                }

                if (string.IsNullOrEmpty(model.ManagerPhoneNumber))
                {
                    missingData.Add("رقم هاتف المدير");
                }

                if (string.IsNullOrEmpty(model.ManagerMobileNumber))
                {
                    missingData.Add("رقم جوال المدير");
                }

                if (missingData.Count > 0)
                {
                    result.BadRequest($" معلومات الاتصال غير كاملة. يرجى التحقق من [{string.Join(", ", missingData)}]");
                    result.Data = false;
                    return result;
                }

                var registerResult = new ReturnResult<WaslResult>();

                var waslUpdateCompanyModel = new WaslUpdateCompanyModel()
                {
                    Activity = fleetDetails.ActivityType,
                    IdentityNumber = fleetDetails.IdentityNumber,
                    CommercialRecordNumber = fleetDetails.CommercialRecordNumber,
                    ManagerName = model.ManagerName,
                    ManagerPhoneNumber = model.ManagerPhoneNumber,
                    ManagerMobileNumber = model.ManagerMobileNumber,
                };

                registerResult = await _waslOperatingCompaniesManager.UpdateContactInfoAsync(waslUpdateCompanyModel);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.UpdateWaslContactInfo, fleetDetails.Id, registerResult, model.UpdatedBy);

                if (registerResult.IsSuccess)
                {
                    fleetDetails.ManagerName = model.ManagerName;
                    fleetDetails.ManagerPhoneNumber = model.ManagerPhoneNumber;
                    fleetDetails.ManagerMobileNumber = model.ManagerMobileNumber;
                    fleetDetails.UpdatedBy = model.UpdatedBy;
                    fleetDetails.UpdatedDate = DateTime.Now;

                    await _unitOfWork.FleetRepository.UpdateFleetDetailsWaslContactInfoAsync(fleetDetails);
                    await _unitOfWork.EventLogRepository.LogEventAsync(Event.UpdateWaslContactInfo, fleetDetails.Id, fleetDetails, model.UpdatedBy);

                    result.Success(true);
                }
                else
                {
                    result.BadRequest(registerResult.ErrorList);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<bool>> UpdateCompanySettingsAysnc(long FleetId, string UpdatedBy, CompanySettingViewModel companySettingView)
        {
            var result = new ReturnResult<bool>();
            try
            {
               var model = await _unitOfWork.FleetRepository.UpdateCompanySettingsAysnc(FleetId, UpdatedBy, companySettingView);

                await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, model.Id, model, model.UpdatedBy);

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

        public async Task<ReturnResult<CompanySettingViewModel>> LoadCompanySettingsAysnc(long FleetId)
        {
            var result = new ReturnResult<CompanySettingViewModel>();
            try
            {
                var fleet = await _unitOfWork.FleetRepository.FindByIdAsync(FleetId);
                if (fleet == null)
                {
                    result.NotFound(_sharedLocalizer["FleetNotExists"]);
                    return result;
                }

                var companySetting = new CompanySettingViewModel();
                if(!string.IsNullOrEmpty(fleet.LogoPhotoExtention))
                {
                    companySetting.LogoPhotoByte = fleet.LogoPhotoByte;
                    companySetting.LogoPhotoExtention = fleet.LogoPhotoExtention;
                    if (fleet.LogoPhotoByte.Length > 0)
                    {
                        companySetting.LogoFileBase64 = Convert.ToBase64String(fleet.LogoPhotoByte);
                    }
                }
                
                result.Success(companySetting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }
    }
}
