using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.ReportsSchedule
{
    public interface IReportScheduleRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        Task<PagedResult<ReportSchedule>> SearchAsync(GenericSearchModel searchModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<ReportSchedule> GetByIdAsync(long Id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<ReportSchedule> GetDetailsByIdAsync(long Id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<ReportSchedule>> GetScheduledReportsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FleetId"></param>
        /// <returns></returns>
        Task<int> CountAsync(long? FleetId = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="UpdatedBy"></param>
        /// <returns></returns>
        Task<ReportSchedule> DeleteAsync(long Id, string UpdatedBy);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="activate"></param>
        /// <param name="UpdatedBy"></param>
        /// <returns></returns>
        Task<ReportSchedule> ActiveStatusAsync(long Id, bool activate, string UpdatedBy);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportSchedule"></param>
        /// <returns></returns>
        Task<ReportSchedule> AddAsync(ReportSchedule reportSchedule);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportScheduleView"></param>
        /// <returns></returns>
        Task<ReportSchedule> UpdateAsync(ReportSchedule reportSchedule);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportScheduleId"></param>
        /// <param name="scheduleTypeEnum"></param>
        /// <returns></returns>
        Task<ReportScheduleHistory> GetReportScheduleHistoryByIdAsync(long reportScheduleId, ScheduleTypeEnum scheduleTypeEnum);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportScheduleHistory"></param>
        /// <returns></returns>
        Task AddReportScheduleHistoryAsync(ReportScheduleHistory reportScheduleHistory);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<ScheduleTypeLookup> GetScheduleTypeLookupByIdAsync(int Id);
    }
}
