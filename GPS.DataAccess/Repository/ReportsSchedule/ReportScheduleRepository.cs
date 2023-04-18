using GPS.DataAccess.Context;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using GPS.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.ReportsSchedule
{
    public class ReportScheduleRepository : IReportScheduleRepository
    {
        private readonly TrackerDBContext _dbContext;

        public ReportScheduleRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<ReportSchedule>> SearchAsync(GenericSearchModel searchModel)
        {
            var pagedList = new PagedResult<ReportSchedule>();

            var skip = (searchModel.PageNumber - 1) * searchModel.PageSize;

            pagedList.TotalRecords = await _dbContext.ReportSchedule.Where(x => !x.IsDeleted &&
            x.UserId == searchModel.UserId &&
            (!searchModel.FleetId.HasValue || x.FleetId == searchModel.FleetId) &&
             (!searchModel.ReportTypeLookupId.HasValue || x.ReportTypeLookupId == searchModel.ReportTypeLookupId) &&
            (string.IsNullOrEmpty(searchModel.SearchString) || x.Name.Contains(searchModel.SearchString)))
                .CountAsync();

            pagedList.List = await _dbContext.ReportSchedule.Where(x => !x.IsDeleted &&
                x.UserId == searchModel.UserId &&
                (!searchModel.FleetId.HasValue || x.FleetId == searchModel.FleetId) &&
                 (!searchModel.ReportTypeLookupId.HasValue || x.ReportTypeLookupId == searchModel.ReportTypeLookupId) &&
                (string.IsNullOrEmpty(searchModel.SearchString) || x.Name.Contains(searchModel.SearchString)))
                     .OrderByDescending(x => x.CreatedDate)
                    .Skip(skip).Take(searchModel.PageSize)
                    .Include(x => x.Fleet)
                    .Include(x => x.ReportTypeLookup)
                    .Include(x => x.DaysOfWeekLookup)
                    .AsNoTracking().ToListAsync();

            return pagedList;
        }

        public async Task<ReportSchedule> GetByIdAsync(long Id)
        {
            var reportSchedule = await _dbContext.ReportSchedule.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
            if (reportSchedule == null)
            {
                return null;
            }

            return reportSchedule;
        }

        public async Task<ReportSchedule> GetDetailsByIdAsync(long Id)
        {
            var reportSchedule = await _dbContext.ReportSchedule.AsNoTracking()
                    .Include(x => x.Warehouse)
                    .Include(x => x.Inventory)
                    .Include(x => x.ReportTypeLookup)
                    .FirstOrDefaultAsync(x => x.Id == Id);

            return reportSchedule;
        }

        public async Task<List<ReportSchedule>> GetScheduledReportsAsync()
        {
            var currentDate = DateTime.Now;
            var currentHour = currentDate.ToString("HH");
            var currentDayOfWeek = (int)currentDate.DayOfWeek;
            var currentDayOfMonth = currentDate.Day;

            GPSHelper.LogHistory($"GPS GetScheduledReportsAsync currentDate: {currentDate}");
            GPSHelper.LogHistory($"GPS GetScheduledReportsAsync currentHour: {currentHour}");
            GPSHelper.LogHistory($"GPS GetScheduledReportsAsync currentDayOfWeek: {currentDayOfWeek}");
            GPSHelper.LogHistory($"GPS GetScheduledReportsAsync currentDayOfMonth: {currentDayOfMonth}");

            var reportSchedules = await _dbContext.ReportSchedule.Where(x => !x.IsDeleted && x.IsActive &&
                ((x.Daily == true && x.DailyTime.Substring(0, 2).Equals(currentHour))
                //|| (x.Weekly == true && x.DayOfWeekId == currentDayOfWeek && x.WeeklyTime.Substring(0, 2).Equals(currentHour)) ||
                //(x.Monthly == true && x.DayOfMonthId == currentDayOfMonth && x.MonthlyTime.Substring(0, 2).Equals(currentHour))

                ))
                    .Include(x => x.ReportTypeLookup)
                    .Include(x => x.DaysOfWeekLookup)
                    .AsNoTracking()
                    .ToListAsync();

            return reportSchedules;
        }

        public async Task<int> CountAsync(long? FleetId = null)
        {
            var count = await _dbContext.ReportSchedule.AsNoTracking().Where(x => !x.IsDeleted && (!FleetId.HasValue || x.FleetId == FleetId)).CountAsync();
            return count;
        }

        public async Task<ReportSchedule> DeleteAsync(long Id, string UpdatedBy)
        {
            var reportSchedule = await _dbContext.ReportSchedule.FindAsync(Id);
            if (reportSchedule == null)
            {
                return null;
            }

            reportSchedule.IsDeleted = true;
            reportSchedule.UpdatedBy = UpdatedBy;
            reportSchedule.UpdatedDate = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            return reportSchedule;
        }

        public async Task<ReportSchedule> ActiveStatusAsync(long Id, bool activate, string UpdatedBy)
        {
            var reportSchedule = await _dbContext.ReportSchedule.FindAsync(Id);
            if (reportSchedule == null)
            {
                return null;
            }

            reportSchedule.IsActive = activate;
            reportSchedule.UpdatedBy = UpdatedBy;
            reportSchedule.UpdatedDate = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            return reportSchedule;
        }

        public async Task<ReportSchedule> AddAsync(ReportSchedule reportSchedule)
        {
            reportSchedule.CreatedDate = DateTime.Now;
            var result = await _dbContext.ReportSchedule.AddAsync(reportSchedule);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<ReportSchedule> UpdateAsync(ReportSchedule reportSchedule)
        {
            var result = _dbContext.ReportSchedule.Update(reportSchedule);
            await _dbContext.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<ReportScheduleHistory> GetReportScheduleHistoryByIdAsync(long reportScheduleId, ScheduleTypeEnum scheduleTypeEnum)
        {
            var reportScheduleHistory = await _dbContext.ReportScheduleHistory
              .AsNoTracking()
              .OrderByDescending(x => x.ReportScheduleId == reportScheduleId && x.ScheduleTypeId == (int)scheduleTypeEnum).FirstOrDefaultAsync();

            if (reportScheduleHistory == null)
            {
                return null;
            }

            return reportScheduleHistory;
        }

        public async Task AddReportScheduleHistoryAsync(ReportScheduleHistory reportScheduleHistory)
        {
            await _dbContext.ReportScheduleHistory.AddAsync(reportScheduleHistory);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ScheduleTypeLookup> GetScheduleTypeLookupByIdAsync(int Id)
        {
            var scheduleTypeLookup = await _dbContext.ScheduleTypeLookup.FindAsync(Id);
            if (scheduleTypeLookup == null)
            {
                return null;
            }

            return scheduleTypeLookup;
        }
    }
}
