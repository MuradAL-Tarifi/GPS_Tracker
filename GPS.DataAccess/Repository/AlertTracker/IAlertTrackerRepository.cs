using GPS.Domain.DTO;
using GPS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.AlertTracker
{
    public interface IAlertTrackerRepository
    {
        Task<PagedResult<GPS.Domain.Models.AlertTracker>> SearchAsync(string warehouseName, string fleetName, string sensorNumber, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize);

    }
}
