using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Alerts
{
    public interface IAlertsRepository
    {
        Task<Alert> AddAsync(Alert alert);
        Task<List<Alert>> GetTop100AlertsAsync(string userId);
        Task<int> NumberOfUnViewedAlerts(string userId);
        Task<PagedResult<Alert>> SearchAsync(string userId, long? warehouseId, long? inventoryId, long? sensorId, long? alertType, long? alertId, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize);
        Task<bool> UpdateAlertsAsReadAsync(List<long> alertIds, string userId);
    }
}
