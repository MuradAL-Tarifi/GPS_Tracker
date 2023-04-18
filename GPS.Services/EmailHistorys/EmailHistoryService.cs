using AutoMapper;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.EmailHistorys
{
    public class EmailHistoryService : IEmailHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EmailHistoryService> _logger;


        public EmailHistoryService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<EmailHistoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;

        }

        //public async Task<ReturnResult<EmailHistoryView>> SearchAsync(long VehicleId)
        //{
        //    var result = new ReturnResult<EmailHistoryView>();
        //    try
        //    {
        //        var emailHistory = await _unitOfWork.EmailHistoryRepository.SearchAsync(VehicleId);
        //        result.Success(_mapper.Map<EmailHistoryView>(emailHistory));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message, result);
        //        result.ServerError(ex.Message);
        //    }
        //    return result;
        //}

        public async Task<ReturnResult<long>> AddAsync(EmailHistoryView emailHistoryView)
        {
            var result = new ReturnResult<long>();
            try
            {
                var entity = _mapper.Map<EmailHistory>(emailHistoryView);
                await _unitOfWork.EmailHistoryRepository.AddAsync(entity);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.create, entity.Id, entity, "System");
                result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
                result.Data = -1;
            }

            return result;
        }

        //public async Task<ReturnResult<bool>> UpdateSentAsync(long id, string UpdatedBy)
        //{
        //    var result = new ReturnResult<bool>();
        //    try
        //    {
        //        bool updated = await _unitOfWork.EmailHistoryRepository.UpdateSentAsync(id);
        //        if (updated)
        //        {
        //            await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, id, id, UpdatedBy);
        //        }
        //        result.Success(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message, result);
        //        result.ServerError(ex.Message);
        //        result.Data = false;
        //    }
        //    return result;
        //}


    }
}
