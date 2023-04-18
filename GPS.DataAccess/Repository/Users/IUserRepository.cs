using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Users
{
   public interface IUserRepository
    {
        Task<PagedResult<GPS.Domain.Models.User>> SearchAsync(int? agentId, long? fleetId,
        long? warehouseId, int? isActive, string searchString, bool canViewSuperAdmin, bool CanViewAgentUsers, int pageNumber, int pageSize);

        Task<GPS.Domain.Models.User> GetByIdAsync(string Id);

        Task<bool> AddWithRoleAsync(GPS.Domain.Models.User user, string roleName, long[] warehouseIds = null, List<long> inventoryIds = null);

        Task<bool> UpdateWithRoleAsync(UserView userView, List<long> inventoryIds = null);

        Task<GPS.Domain.Models.User> DeleteAsync(string id, string updatedBy);

        Task<int> CountAsync();

        Task<bool> IsUsernameExistsAsync(string Username);

        Task<bool> IsEmailExistsAsync(string Email);

        Task<List<Role>> GetRolesAsync();

        Task<List<UserPrivilege>> GetUserPrivilegesAsync(string userId);

        Task<bool> SaveUserPrivilegesAsync(string UserId, List<UserPrivilegeView> UserPrivileges, string UpdatedBy);

        Task<UserProfile> Login(UserLoginModel loginModel);

        Task<List<int>> GetActivePrivilegeTypeIdsAsync(string userId);

        Task<GPS.Domain.Models.User> FindAsync(string Id, string Username);

        Task<GPS.Domain.Models.User> GetByUserNameAndPasswordAsync(string username, string password);

        Task<Role> GetRoleByUserIdAsync(string userId);

        Task<List<LookupModel>> GetUserWarehousesAsync(string userId);
        Task<List<Inventory>> GetUserInventoriesAsync(string userId);

        /// <summary>
        /// toggle user mobile alerts
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> ToggleMobileAlerts(UserToggleAlertsModel model);

        Task<DateTime?> CurrentUserExpirationDateAsync(string currentUserId);


        Task<List<User>> GetUsersByFleetId(long fleetId);
        Task<List<Inventory>> GetUserInventoriesAndWarehouesAsync(string userId);

        Task<List<User>> GetUsersByIdsAsync(List<string> userIds);
    }
}
