using AutoMapper;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Views;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.EventLogs
{
    public class EventLogService : IEventLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EventLogService> _logger;

        public EventLogService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<EventLogService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task LogEventAsync(Domain.DTO.Event type, Object objectId, Object data, string userId)
        {
            try
            {
                await _unitOfWork.EventLogRepository.LogEventAsync(type, objectId, data, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task<ReturnResult<PagedResult<EventLogView>>> SearchAsync(string type, DateTime? fromDate = null, DateTime? toDate = null, string searchString = "", int pageNumber = 1, int pageSize = 100)
        {
            var result = new ReturnResult<PagedResult<EventLogView>>();
            try
            {
                var pagedResult = await _unitOfWork.EventLogRepository.SearchAsync(type, fromDate, toDate, searchString, pageNumber, pageSize);

                var pagedListView = new PagedResult<EventLogView>
                {
                    TotalRecords = pagedResult.TotalRecords,
                    List = _mapper.Map<List<EventLogView>>(pagedResult.List)
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
    }
}
