using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Fleets
{
    public interface IFleetService
    {
        /// <summary>
        /// Get Fleets, Get Fleets by AgentId, Search Fleets by Name/NameEn
        /// </summary>
        /// <param name="AgentId"></param>
        /// <param name="SearchString"></param>
        /// <param name="PageNumber"></param>
        /// <param name="PageSize"></param>
        /// <returns>List of Fleets</returns>
        Task<ReturnResult<PagedResult<FleetView>>> SearchAsync(string SearchString = "", int? waslLinkStatus = null, int PageNumber = 1, int pageSize = 100);

        /// <summary>
        /// Find Fleet by Id
        /// </summary>
        Task<ReturnResult<FleetView>> FindByIdAsync(long? Id);

        /// <summary>
        /// Add Fleet
        /// </summary>
        /// <param name="fleet"></param>
        Task<ReturnResult<bool>> AddAsync(FleetView fleet);

        /// <summary>
        /// Update Fleet
        /// </summary>
        /// <param name="fleet"></param>
        Task<ReturnResult<bool>> UpdateAsync(FleetView fleet);

        /// <summary>
        /// Delete a Fleet
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="UpdatedBy"></param>
        Task<ReturnResult<bool>> DeleteAsync(long Id, string UpdatedBy);

        /// <summary>
        /// Get Fleets Count
        /// </summary>
        Task<ReturnResult<int>> CountAsync();

        /// <summary>
        /// Get Linked With Wasl Fleets Count
        /// </summary>
        Task<ReturnResult<int>> CountLinkedWithWaslAsync();

        /// <summary>
        /// Get All Fleetes
        /// </summary>
        /// <param name="AgentId"></param>
        Task<ReturnResult<List<FleetView>>> GetAllAsync(int? AgentId = null);

        /// <summary>
        /// Get All FleetesWASL
        /// </summary>
        /// <param name="AgentId"></param>
        Task<ReturnResult<List<FleetView>>> GetFleetsWASLAsync(int? AgentId = null);

        /// <summary>
        /// Check if Name is exists
        /// </summary>
        /// <param name="AgentId"></param>
        /// <param name="Name"></param>
        Task<bool> IsNameExistsAsync(int AgentId, string Name);

        /// <summary>
        /// Check if NameEn is exists
        /// </summary>
        /// <param name="AgentId"></param>
        /// <param name="NameEn"></param>
        /// <returns></returns>
        Task<bool> IsNameEnExistsAsync(int AgentId, string NameEn);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<ReturnResult<FleetDetailsView>> FindDetailsByIdAsync(long? Id);

        Task<ReturnResult<bool>> LinkWithWasl(long id, string updatedBy);

        Task<ReturnResult<bool>> UnlinkWithWasl(long id, string updatedBy);

        /// <summary>
        /// Get Wasl Details 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<ReturnResult<FleetWaslModel>> GetWaslDetailsAsync(long? fleetId);

        /// <summary>
        /// Update Wasl Details 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ReturnResult<bool>> UpdateWaslDetailsAsync(FleetWaslModel model);

        /// <summary>
        /// Update Wasl Details from Agent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ReturnResult<bool>> AgentUpdateWaslDetailsAsync(FleetWaslModel model);

        /// <summary>
        /// Update Wasl Contact Info
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        Task<ReturnResult<bool>> UpdateWaslContactInfoAsync(FleetWaslModel model);

        /// <summary>
        /// Update Company Settings Such As Logo
        /// </summary>
        /// <param name="FleetId"></param>
        /// <param name="updatedBy"></param>
        /// <param name="companySettingView"></param>
        /// <returns></returns>
        Task<ReturnResult<bool>> UpdateCompanySettingsAysnc(long FleetId, string UpdatedBy, CompanySettingViewModel companySettingView);

        /// <summary>
        /// Load Company Setting Such As Logo
        /// </summary>
        /// <param name="FleetId"></param>
        /// <returns></returns>
        Task<ReturnResult<CompanySettingViewModel>> LoadCompanySettingsAysnc(long FleetId);
    }
}
