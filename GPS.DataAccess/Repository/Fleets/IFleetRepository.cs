using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Fleets
{
   public interface IFleetRepository
    {
        Task<PagedResult<Fleet>> SearchAsync(string searchString, int? waslLinkStatus, int pageNumber, int pageSize);

        Task<Fleet> FindByIdAsync(long? Id);

        Task<FleetDetails> FindDetailsByFleetIdAsync(long? Id);

        Task<Fleet> AddAsync(Fleet fleet);

        Task<bool> UpdateAsync(FleetView model);

        Task<Fleet> DeleteAsync(long Id, string UpdatedBy);

        Task<FleetDetails> UpdateLinkedWithWaslInfoAsync(FleetDetails fleetDetails);
        Task<bool> AddDefualtFleetDetailsAsync(FleetDetails fleetDetails);
        Task<bool> DeleteFleetDetailsAsync(long fleetId);
        Task<int> CountAsync();

        Task<int> CountLinkedWithWaslAsync();

        Task<List<Fleet>> GetAllAsync();

        Task<List<Fleet>> GetFleetsWASLAsync(int? AgentId);

        Task<bool> IsNameExistsAsync(int AgentId, string Name);

        Task<bool> IsNameEnExistsAsync(int AgentId, string NameEn);

        Task<bool> UpdateWaslDetailsAsync(FleetWaslModel model);

        Task<List<FleetDetails>> GetDetailsListByFleetIdAsync(long? fleetId);

        Task<bool> AgentUpdateWaslDetailsAsync(FleetWaslModel model);

        Task<bool> IsAnyLinkedWithWaslAsync(long fleetId);

        Task<FleetDetails> UpdateFleetDetailsWaslContactInfoAsync(FleetDetails fleetDetails);

        Task<Fleet> UpdateCompanySettingsAysnc(long FleetId, string updatedBy, CompanySettingViewModel companySettingView);
    }
}
