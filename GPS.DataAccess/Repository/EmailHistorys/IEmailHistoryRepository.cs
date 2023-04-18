using GPS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.EmailHistorys
{
    public interface IEmailHistoryRepository
    {
        //Task<EmailHistory> SearchAsync(long VehicleId);

        //Task<EmailHistory> GetForVehicleCcustomAlertAsync(long VehicleId);

        //Task<EmailHistory> GetForInventorySensorCcustomAlertAsync(long inventorySensorId);

        Task<long> AddAsync(EmailHistory entity);

        //Task<bool> UpdateSentAsync(long id);
    }
}
