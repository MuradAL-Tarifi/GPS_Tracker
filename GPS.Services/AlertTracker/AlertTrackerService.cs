using AutoMapper;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Helper;
using GPS.Resources;
using GPS.Services.Agents;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GPS.Services.AlertTracker
{
    public class AlertTrackerService : IAlertTrackerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AgentService> _logger;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public AlertTrackerService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AgentService> logger,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
        }
        public async Task<ReturnResult<PagedResult<AlertTrackerViewModel>>> GetPagedAlertsTrackerAsync(string warehouseName, string fleetName, string sensorNumber, string fromDate, string toDate, int pageNumber, int pageSize)
        {
            var result = new ReturnResult<PagedResult<AlertTrackerViewModel>>();
            try
            {
                DateTime? FromDate = null;
                DateTime? ToDate = null;
                fromDate = fromDate != "null" ? fromDate : "";
                toDate = toDate != "null" ? toDate : "";
                if (pageSize == 999999999)
                {
                    if (!string.IsNullOrEmpty(fromDate))
                    {
                        FromDate = Convert.ToDateTime(fromDate);
                    }
                    if (!string.IsNullOrEmpty(toDate))
                    {
                        ToDate = Convert.ToDateTime(toDate);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(fromDate))
                    {
                        FromDate = GPSHelper.StringToDateTime(fromDate);
                    }
                    if (!string.IsNullOrEmpty(toDate))
                    {
                        ToDate = GPSHelper.StringToDateTime(toDate);
                    }
                }

                var pagedAlerts = await _unitOfWork.AlertTrackerRepository.SearchAsync(warehouseName, fleetName, sensorNumber, FromDate, ToDate, pageNumber, pageSize);
                //var lsAlertViewModel = await BindToAlertTrackerViewModelAsync(_mapper.Map<List<AlertView>>(pagedAlerts.List));

                var pagedListView = new PagedResult<AlertTrackerViewModel>
                {
                    TotalRecords = pagedAlerts.TotalRecords,
                    List = _mapper.Map<List<Domain.Models.AlertTracker>, List<AlertTrackerViewModel>>(pagedAlerts.List)
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
