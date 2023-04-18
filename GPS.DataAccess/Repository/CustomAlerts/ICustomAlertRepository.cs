using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.CustomAlerts
{
   public interface ICustomAlertRepository
    {
        Task<List<CustomAlert>> SearchAsync(long FleetId, long? WarehouseId = null, long? InventoryId = null, int? IsActive = null, string SearchString = "");
        Task<List<Inventory>> GetCustomAlertInventoriesAsync(long CustomAlertId);
        Task<CustomAlert> AddAsync(CustomAlert CustomAlert, long[] InvertoryIds);
        Task<CustomAlert> UpdateAsync(CustomAlert CustomAlert, long[] InvertoryIds);
        Task<CustomAlert> DeleteAsync(long Id, string UpdatedBy);
        Task<CustomAlert> FindAsync(long CustomAleryId);
        Task<List<CustomAlert>> GetAllActiveAsync();
    }
}
