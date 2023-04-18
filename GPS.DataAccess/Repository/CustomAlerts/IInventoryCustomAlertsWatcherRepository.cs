using GPS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.CustomAlerts
{
   public interface IInventoryCustomAlertsWatcherRepository
    {
        Task<CustomAlertWatcher> GetLastAsync();
        Task AddAsync(DateTime fromDate, DateTime toDate);
    }
}
