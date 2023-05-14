using DXApplicationFDA.Infra.Services;
using GPS.API.Proxy;
using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Helper;
using GPS.Services.Alerts;
using GPS.Services.Lookups;
using GPS.Services.Sensors;
using GPS.Web.Agent.AppCode;
using GPS.Web.Agent.AppCode.Helpers;
using GPS.Web.Agent.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace GPS.Web.Agent.Controllers
{
    public class AlarmController : BaseController
    {
        private readonly IInventoryHistoryReportApiProxy _inventoryHistoryReportApiProxy;
        private readonly IOnlineInventoryHistoryApiProxy _onlineInventoryHistoryApiProxy;
        private readonly IAlertApiProxy _alertApiProxy;
        private readonly ISensorService _SensorService;
        public AlarmController(
            IInventoryHistoryReportApiProxy inventoryHistoryReportApiProxy,
            IOnlineInventoryHistoryApiProxy onlineInventoryHistoryApiProxy,
            IViewHelper viewHelper,
            LoggedInUserProfile loggedUser,
            ISensorService SensorService,
            IAlertApiProxy alertApiProxy,
            ILookupsService lookupsService) : base(viewHelper, loggedUser, lookupsService)
        {
            _inventoryHistoryReportApiProxy = inventoryHistoryReportApiProxy;
            _onlineInventoryHistoryApiProxy = onlineInventoryHistoryApiProxy;
            _alertApiProxy = alertApiProxy;
            _SensorService = SensorService;
        }


    }
}
