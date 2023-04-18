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
using System.Threading.Tasks;

namespace GPS.Services.Gateways
{
    public class GatewayService : IGatewayService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GatewayService> _logger;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public GatewayService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GatewayService> logger,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
        }

        public async Task<ReturnResult<PagedResult<GatewayView>>> SearchAsync(string SearchString = "", int PageNumber = 1, int pageSize = 100)
        {
            var result = new ReturnResult<PagedResult<GatewayView>>();
            try
            {
                var pagedResult = await _unitOfWork.GatewayRepository.SearchAsync(SearchString, PageNumber, pageSize);

                var pagedListView = new PagedResult<GatewayView>
                {
                    TotalRecords = pagedResult.TotalRecords,
                    List = _mapper.Map<List<GatewayView>>(pagedResult.List)
                };

                result.Success(pagedListView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<GatewayView>> FindByIdAsync(long? Id)
        {
            var result = new ReturnResult<GatewayView>();
            try
            {
                var gateway = await _unitOfWork.GatewayRepository.FindByIdAsync(Id);
                if (gateway == null)
                {
                    result.NotFound(_sharedLocalizer["GatewayNotExists"]);
                    return result;
                }
                var brand = await _unitOfWork.BrandRepository.FindbyIdAsync(gateway.BrandId);
                gateway.Brand = brand;
                result.Success(_mapper.Map<GatewayView>(gateway));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<bool>> AddAsync(GatewayView gateway)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var newGateway = _mapper.Map<Gateway>(gateway);
                newGateway.CreatedBy = gateway.CreatedBy;
                newGateway.CreatedDate = DateTime.Now;
                newGateway.IMEI = gateway.IMEI.ToLower();
                await _unitOfWork.GatewayRepository.AddAsync(newGateway);

                await _unitOfWork.EventLogRepository.LogEventAsync(Event.create, newGateway.Id, newGateway, gateway.CreatedBy);

                result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }

            return result;
        }

        public async Task<ReturnResult<bool>> UpdateAsync(GatewayView model)
        {
            var result = new ReturnResult<bool>();
            try
            {
                model.IMEI = model.IMEI.ToLower();
                await _unitOfWork.GatewayRepository.UpdateAsync(model);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, model.Id, model, model.UpdatedBy);
                result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<bool>> DeleteAsync(long Id, string UpdatedBy)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var gateway = await _unitOfWork.GatewayRepository.DeleteAsync(Id, UpdatedBy);
                if (gateway == null)
                {
                    result.NotFound(_sharedLocalizer["GatewayNotExists"]);
                    result.Data = false;
                    return result;
                }

                await _unitOfWork.EventLogRepository.LogEventAsync(Event.delete, gateway.Id, gateway, UpdatedBy);

                result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<bool> IsNameExistsAsync(string Name)
        {
            try
            {
                var exists = await _unitOfWork.GatewayRepository.IsNameExistsAsync(Name);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return false;
        }

        public async Task<bool> IsIMEIExistsAsync(string IMEI)
        {
            try
            {
                var exists = await _unitOfWork.GatewayRepository.IsIMEIExistsAsync(IMEI);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return false;
        }
        public async Task<bool> IsGatewayLinkedToInventoryAsync(long gatewayId)
        {
            try
            {
                var exists = await _unitOfWork.InventoryRepository.IsGatewayRegisteredWithInventoryAsync(gatewayId);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return false;
        }
        
    }
}
