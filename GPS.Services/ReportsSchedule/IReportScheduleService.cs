using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.ReportsSchedule
{
    public interface IReportScheduleService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        Task<ReturnResult<PagedResult<ReportScheduleView>>> SearchAsync(GenericSearchModel searchModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportOptions"></param>
        /// <returns></returns>
        Task<ReturnResult<bool>> SaveAsync(ReportOptionsModel reportOptions);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="UpdatedBy"></param>
        /// <returns></returns>
        Task<ReturnResult<bool>> DeleteAsync(long Id, string UpdatedBy);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FleetId"></param>
        /// <returns></returns>
        Task<ReturnResult<int>> CountAsync(long? FleetId = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<ReturnResult<ReportOptionsModel>> GetByIdAsync(long Id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ReturnResult<ReportOptionsDetailsModel>> GetReportDetailsAsync(long id);

        /// <summary>
        /// Active Status Async
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="activate"></param>
        /// <param name="UpdatedBy"></param>
        /// <returns></returns>
        Task<ReturnResult<bool>> ActiveStatusAsync(long Id, bool activate, string UpdatedBy);

        /// <summary>
        /// temp
        /// </summary>
        /// <returns></returns>
        //Task GenerateAndSendGeoFenceReport();       
    }
}
