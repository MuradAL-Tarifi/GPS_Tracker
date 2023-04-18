using AutoMapper;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Helper;
using GPS.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Users
{
   public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public UserService(
             IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UserService> logger,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
        }

        public async Task<ReturnResult<PagedResult<UserView>>> SearchAsync(int? AgentId = null, long? FleetId = null,
             long? warehouseId = null, int? IsActive = null, string SearchString = "", bool CanViewSuperAdmin = false, bool CanViewAgentUsers = false, int PageNumber = 1, int PageSize = 100)
        {
            var result = new ReturnResult<PagedResult<UserView>>();
            try
            {
                var pagedResult = await _unitOfWork.UserRepository.SearchAsync(AgentId, FleetId, warehouseId, IsActive, SearchString, CanViewSuperAdmin, CanViewAgentUsers, PageNumber, PageSize);

                var pagedListView = new PagedResult<UserView>
                {
                    TotalRecords = pagedResult.TotalRecords,
                    List = _mapper.Map<List<UserView>>(pagedResult.List)
                };

                result.Success(pagedListView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<UserView>> FindAsync(string Id, string Username = "")
        {
            var result = new ReturnResult<UserView>();

            try
            {
                var user = await _unitOfWork.UserRepository.FindAsync(Id, Username);

                if (user == null || user.IsDeleted)
                {
                    result.NotFound(_sharedLocalizer["UserNotExists"]);
                    return result;
                }

                var userView = _mapper.Map<UserView>(user);
                var Role = await _unitOfWork.UserRepository.GetRoleByUserIdAsync(user.Id);

                var wareHouses = await _unitOfWork.UserRepository.GetUserWarehousesAsync(user.Id);
                if (wareHouses.Count > 0)
                {
                    userView.WarehouseIds = wareHouses.Select(x => x.Id).ToArray();
                    userView.WarehouseNames = wareHouses.Select(x => x.Name).ToArray();
                }

                var userInventories = await _unitOfWork.UserRepository.GetUserInventoriesAsync(user.Id);
                var inventoryIds = userInventories.Select(x =>x.Id).ToList();
                userView.InventoriesIds = inventoryIds.Count > 0 ? string.Join(",", inventoryIds): null;

                userView.Role = _mapper.Map<RoleView>(Role);

                result.Success(userView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<UserView>> GetByIdAsync(string Id)
        {
            var result = new ReturnResult<UserView>();

            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(Id);

                if (user == null || user.IsDeleted)
                {
                    result.NotFound(_sharedLocalizer["UserNotExists"]);
                    return result;
                }

                result.Success(_mapper.Map<UserView>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<bool>> SaveAsync(UserView user)
        {
            user.FleetId = user.FleetId <= 0 ? null : user.FleetId;

            if (!string.IsNullOrEmpty(user.Id))
            {
                return await UpdateAsync(user);
            }
            else
            {
                return await AddAsync(user);
            }
        }

        private async Task<ReturnResult<bool>> AddAsync(UserView userView)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var inventoryIds = !string.IsNullOrEmpty(userView.InventoriesIds) ? userView.InventoriesIds.Split(",").ToList().ConvertAll(x => Int64.Parse(x)): new List<long>();
                var user = _mapper.Map<User>(userView);
                user.CreatedBy = userView.CreatedBy;
                user.CreatedDate = DateTime.Now;
                user.Password = userView.Password;
                user.IsAdmin = userView.Role.Name.Equals(Roles.administrator.ToString());
                user.Id = Guid.NewGuid().ToString();
                if (userView.Role.Name.Equals(Roles.administrator.ToString()))
                {
                    user.FleetId = null;
                }
                else
                {
                    // check if it is first added user for agent
                    userView = await HandelUserSubAdminAgentAsync(userView);
                    user.IsSubAdminAgent = userView.IsSubAdminAgent;
                    user.ExpirationDate = userView.ExpirationDate;
                }
                await _unitOfWork.UserRepository.AddWithRoleAsync(user, userView.Role.Name, userView.WarehouseIds, inventoryIds);

                await _unitOfWork.EventLogRepository.LogEventAsync(Event.create, user.Id, user, userView.CreatedBy);

                result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        private async Task<UserView> HandelUserSubAdminAgentAsync(UserView userView)
        {
            var users = await _unitOfWork.UserRepository.GetUsersByFleetId((long)userView.FleetId);
            if (users.Count > 0)
            {
                if (users.Any(x => x.IsSubAdminAgent))
                {
                    userView.ExpirationDate = users.Where(x => x.IsSubAdminAgent).Select(x => x.ExpirationDate).FirstOrDefault();
                }
                else
                {
                    userView.IsSubAdminAgent = true;
                }
            }
            else
            {
                // first user agent considered as sub admin user
                userView.IsSubAdminAgent = true;
            }
            return userView;
        }
        private async Task<ReturnResult<bool>> UpdateAsync(UserView userView)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var inventoryIds = !string.IsNullOrEmpty(userView.InventoriesIds) ? userView.InventoriesIds.Split(",").ToList().ConvertAll(x => Int64.Parse(x)) : new List<long>();
                if (userView.Role.Name.Equals(Roles.administrator.ToString()))
                {
                    userView.FleetId = null;
                }
                else
                {
                    // check if it is first added user for agent
                    userView = await HandelUserSubAdminAgentAsync(userView);
                }
                await _unitOfWork.UserRepository.UpdateWithRoleAsync(userView, inventoryIds);

                await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, userView.Id, userView, userView.UpdatedBy);

                result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
                result.Data = false;
            }
            return result;
        }

        public async Task<ReturnResult<bool>> DeleteAsync(string Id, string UpdatedBy)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var user = await _unitOfWork.UserRepository.DeleteAsync(Id, UpdatedBy);

                await _unitOfWork.EventLogRepository.LogEventAsync(Event.delete, user.Id, user, UpdatedBy);

                result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
                result.Data = false;
            }
            return result;
        }

        public async Task<ReturnResult<int>> CountAsync()
        {
            var result = new ReturnResult<int>();
            try
            {
                int Count = await _unitOfWork.UserRepository.CountAsync();
                result.Success(Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<bool> IsUsernameExistsAsync(string Username)
        {
            try
            {
                var exists = await _unitOfWork.UserRepository.IsUsernameExistsAsync(Username);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return false;
        }

        public async Task<bool> IsEmailExistsAsync(string Email)
        {
            try
            {
                var exists = await _unitOfWork.UserRepository.IsEmailExistsAsync(Email);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return false;
        }

        public async Task<ReturnResult<List<RoleView>>> GetRolesAsync()
        {
            var result = new ReturnResult<List<RoleView>>();

            try
            {
                var roles = await _unitOfWork.UserRepository.GetRolesAsync();
                result.Success(_mapper.Map<List<RoleView>>(roles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<UserPrivilegesViewModel>> GetUserPrivilegesAsync(string userId)
        {
            var result = new ReturnResult<UserPrivilegesViewModel>();
            try
            {
                var userPrivilegesViewModel = new UserPrivilegesViewModel();

                userPrivilegesViewModel.User = _mapper.Map<UserView>(await _unitOfWork.UserRepository.GetByIdAsync(userId));
                userPrivilegesViewModel.Privileges = _mapper.Map<List<UserPrivilegeView>>(await _unitOfWork.UserRepository.GetUserPrivilegesAsync(userId));
                result.Success(userPrivilegesViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<bool>> SaveUserPrivilegesAsync(string UserId, List<UserPrivilegeView> UserPrivileges, string UpdatedBy)
        {
            var result = new ReturnResult<bool>();
            try
            {
                await _unitOfWork.UserRepository.SaveUserPrivilegesAsync(UserId, UserPrivileges, UpdatedBy);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, UserId, UserPrivileges, UpdatedBy);

                result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<UserProfile>> Login(UserLoginModel loginModel)
        {
            var result = new ReturnResult<UserProfile>();
            try
            {
                var userProfile = await _unitOfWork.UserRepository.Login(loginModel);

                if (userProfile == null)
                {
                    result.NotFound(_sharedLocalizer["UsernameOrPasswordInValid"]);
                }
                else
                {
                    result.Success(userProfile);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<int>>> GetActivePrivilegeTypeIdsAsync(string userId)
        {
            var result = new ReturnResult<List<int>>();
            try
            {
                var userPrivilegesTypeIds = await _unitOfWork.UserRepository.GetActivePrivilegeTypeIdsAsync(userId);
                result.Success(userPrivilegesTypeIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<UserView>> GetByUserNameAndPasswordAsync(string username, string password)
        {
            var result = new ReturnResult<UserView>();

            try
            {
                var user = await _unitOfWork.UserRepository.GetByUserNameAndPasswordAsync(username, password);

                if (user == null || user.IsDeleted)
                {
                    result.NotFound(_sharedLocalizer["UserNotExists"]);
                    return result;
                }

                var userView = _mapper.Map<UserView>(user);
                var Role = await _unitOfWork.UserRepository.GetRoleByUserIdAsync(user.Id);
                userView.Role = _mapper.Map<Role, RoleView>(Role);

                result.Success(userView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<LookupModel>>> GetUserWarehousesAsync(string userId)
        {
            var result = new ReturnResult<List<LookupModel>>();
            try
            {
                var data = await _unitOfWork.UserRepository.GetUserWarehousesAsync(userId);
                result.Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<InventoryView>>> GetUserInventoriesAndWarehouesAsync(string userId)
        {
            var result = new ReturnResult<List<InventoryView>>();
            try
            {
                var data = await _unitOfWork.UserRepository.GetUserInventoriesAndWarehouesAsync(userId);
                result.Success(_mapper.Map<List<InventoryView>>(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<bool>> ToggleMobileAlerts(UserToggleAlertsModel model)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var updated = await _unitOfWork.UserRepository.ToggleMobileAlerts(model);
                result.Success(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<string> ValidNewUserExpirationDateAsync(DateTime newUserExpirationDate, string currentUserId)
        {
            try
            {
                var currentUserExpirationDate = await _unitOfWork.UserRepository.CurrentUserExpirationDateAsync(currentUserId);
                if (currentUserExpirationDate.HasValue)
                {
                    if (newUserExpirationDate > currentUserExpirationDate)
                    {
                        return GPSHelper.DateToFormatedString(currentUserExpirationDate.Value);
                    }
                    
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return string.Empty;
        }
    }
}
