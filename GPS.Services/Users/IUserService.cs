using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Users
{
   public interface IUserService
    {
        /// <summary>
        /// Search users
        /// </summary>
        /// <param name="AgentId"></param>
        /// <param name="FleetId"></param>
        /// <param name="GroupId"></param>
        /// <param name="AccountId"></param>
        /// <param name="IsActive"></param>
        /// <param name="SearchString"></param>
        /// <param name="PageNumber"></param>
        /// <param name="PageSize"></param>
        Task<ReturnResult<PagedResult<UserView>>> SearchAsync(int? AgentId = null, long? FleetId = null,
             long? warehouseId = null, int? IsActive = null, string SearchString = "",bool CanViewSuperAdmin = false, bool CanViewAgentUsers = false, int PageNumber = 1, int PageSize = 100);

        /// <summary>
        /// Find User by Id or Username
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Username"></param>
        Task<ReturnResult<UserView>> FindAsync(string Id, string Username = "");

        /// <summary>
        /// Add or Update User
        /// </summary>
        /// <param name="user"></param>
        Task<ReturnResult<bool>> SaveAsync(UserView user);

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="UpdatedBy"></param>
        Task<ReturnResult<bool>> DeleteAsync(string Id, string UpdatedBy);

        /// <summary>
        /// Count all Users
        /// </summary>
        /// <returns></returns>
        Task<ReturnResult<int>> CountAsync();

        /// <summary>
        /// Check if Username exists
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        Task<bool> IsUsernameExistsAsync(string Username);

        /// <summary>
        /// Check if Email exists
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        Task<bool> IsEmailExistsAsync(string Email);

        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        Task<ReturnResult<List<RoleView>>> GetRolesAsync();

        /// <summary>
        /// Get User Privileges
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ReturnResult<UserPrivilegesViewModel>> GetUserPrivilegesAsync(string userId);

        /// <summary>
        /// add or update User Privileges
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UserPrivileges"></param>
        /// <returns></returns>
        Task<ReturnResult<bool>> SaveUserPrivilegesAsync(string UserId, List<UserPrivilegeView> UserPrivileges, string UpdatedBy);

        /// <summary>
        /// تسجيل دخول المستخدم
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        Task<ReturnResult<UserProfile>> Login(UserLoginModel loginModel);

        /// <summary>
        /// صلاحيات المستخدم
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ReturnResult<List<int>>> GetActivePrivilegeTypeIdsAsync(string userId);

        /// <summary>
        /// Get User By User Name And Password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<ReturnResult<UserView>> GetByUserNameAndPasswordAsync(string username, string password);

        /// <summary>
        /// Get User By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<ReturnResult<UserView>> GetByIdAsync(string Id);

        /// <summary>
        /// Get User Warehouses
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ReturnResult<List<LookupModel>>> GetUserWarehousesAsync(string userId);

        /// <summary>
        /// toggle user mobile alerts
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ReturnResult<bool>> ToggleMobileAlerts(UserToggleAlertsModel model);

        /// <summary>
        /// Check if Email exists
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        Task<string> ValidNewUserExpirationDateAsync(DateTime newUserExpirationDate, string currentUserId);

        /// <summary>
        /// Get User Inventories
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ReturnResult<List<InventoryView>>> GetUserInventoriesAndWarehouesAsync(string userId);

    }
}
