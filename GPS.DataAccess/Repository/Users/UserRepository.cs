using GPS.DataAccess.Context;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Users
{
   public class UserRepository : IUserRepository
    {
        private readonly TrackerDBContext _dbContext;

        public UserRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<GPS.Domain.Models.User>> SearchAsync(int? agentId, long? fleetId, long? warehouseId, int? isActive, string searchString, bool canViewSuperAdmin,bool canViewAgentUsers, int pageNumber, int pageSize)
        {
            bool active = isActive == 1;
            var pagedList = new PagedResult<GPS.Domain.Models.User>();

            var skip = (pageNumber - 1) * pageSize;

            pagedList.TotalRecords = await _dbContext.User.Where(x => !x.IsDeleted && x.AgentId == agentId &&
            (!fleetId.HasValue || x.FleetId == fleetId) &&
            (!isActive.HasValue || x.IsActive == active) &&
            ((!canViewSuperAdmin && !x.IsSuperAdmin) || canViewSuperAdmin) &&
            ((x.IsAdmin || x.IsSubAdminAgent) || canViewAgentUsers) &&
            (string.IsNullOrEmpty(searchString) || (x.UserName.Contains(searchString) || x.Name.Contains(searchString)
            || x.NameEn.Contains(searchString) || x.Email.Contains(searchString) || x.ExpirationDate.Value.ToString().Contains(searchString))))
                .CountAsync();

            pagedList.List = await _dbContext.User.Where(x => !x.IsDeleted && x.AgentId == agentId &&
                    (!fleetId.HasValue || x.FleetId == fleetId) &&
                    (!isActive.HasValue || x.IsActive == active) &&
                    ((!canViewSuperAdmin && !x.IsSuperAdmin) || canViewSuperAdmin) &&
                    ((x.IsAdmin || x.IsSubAdminAgent) || canViewAgentUsers) &&
                (string.IsNullOrEmpty(searchString) || (x.UserName.Contains(searchString) || x.Name.Contains(searchString)
                || x.NameEn.Contains(searchString) || x.Email.Contains(searchString) || x.ExpirationDate.Value.ToString().Contains(searchString))))
                    .OrderByDescending(x => x.CreatedDate)
                    .Skip(skip).Take(pageSize)
                    .Include(x => x.Agent)
                    .Include(x => x.Fleet)
                    .AsNoTracking().ToListAsync();

            return pagedList;
        }

        public async Task<GPS.Domain.Models.User> GetByIdAsync(string Id)
        {
            var user = await _dbContext.User.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == Id);
            return user;
        }

        public async Task<bool> AddWithRoleAsync(GPS.Domain.Models.User user, string roleName, long[] warehouseIds = null, List<long> inventoryIds = null)
        {
            var addedUser = await _dbContext.User.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var Role = await _dbContext.Role.FirstOrDefaultAsync(x => x.Name == roleName);

            // add role
            await _dbContext.UserRole.AddAsync(new UserRole()
            {
                UserId = addedUser.Entity.Id,
                RoleId = Role.Id
            });
            await _dbContext.SaveChangesAsync();

            if (warehouseIds != null)
            {
                // 0 all warehouses && GroupId == null indicates all groups
                if (warehouseIds.First() == 0)
                {
                    Array.Clear(warehouseIds, 0, warehouseIds.Length);
                    // we query all warehouses based on user's fleet id
                    warehouseIds = await _dbContext.Warehouse.Where(x => x.FleetId == user.FleetId).Select(x => x.Id).ToArrayAsync();
                }
                foreach (var item in warehouseIds)
                {
                    await _dbContext.UserWarehouse.AddAsync(new UserWarehouse()
                    {
                        UserId = addedUser.Entity.Id,
                        WarehouseId = item
                    });
                }
            }
            await _dbContext.SaveChangesAsync();
            if (inventoryIds != null)
            {
                foreach (var item in inventoryIds)
                {
                    await _dbContext.UserInventory.AddAsync(new UserInventory()
                    {
                        UserId = addedUser.Entity.Id,
                        InventoryId = item
                    });
                }
            }
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateWithRoleAsync(UserView userView, List<long> inventoryIds = null)
        {
            var user = await _dbContext.User.FindAsync(userView.Id);
            if (user == null)
            {
                return false;
            }

            var userRole = await _dbContext.UserRole.FirstOrDefaultAsync(x => x.UserId == user.Id);

            var oldRole = await _dbContext.Role.FirstOrDefaultAsync(x => x.Id == userRole.RoleId);

            if (!userView.Role.Name.Equals(oldRole.Name))
            {
                // update Role
                var newRole = await _dbContext.Role.FirstOrDefaultAsync(x => x.Name == userView.Role.Name);

                userRole.RoleId = newRole.Id;
                _dbContext.UserRole.Update(userRole);
                await _dbContext.SaveChangesAsync();

                _dbContext.UserPrivilege.RemoveRange(_dbContext.UserPrivilege.Where(x => x.UserId == userView.Id));
                await _dbContext.SaveChangesAsync();
            }

            var oldUserWarehouses = _dbContext.UserWarehouse.Where(x => x.UserId == user.Id);
            if (oldUserWarehouses.Any())
            {
                _dbContext.UserWarehouse.RemoveRange(oldUserWarehouses);
                await _dbContext.SaveChangesAsync();
            }

            var oldUserInventories = _dbContext.UserInventory.Where(x => x.UserId == user.Id);
            if (oldUserInventories.Any())
            {
                _dbContext.UserInventory.RemoveRange(oldUserInventories);
                await _dbContext.SaveChangesAsync();
            }

            if (userView.WarehouseIds != null)
            {
                // 0 all warehouses && GroupId == null indicates all groups
                if (userView.WarehouseIds.First() == 0)
                {
                    Array.Clear(userView.WarehouseIds, 0, userView.WarehouseIds.Length);
                    // we query all warehouses based on user's fleet id
                    userView.WarehouseIds = await _dbContext.Warehouse.Where(x => x.FleetId == user.FleetId).Select(x => x.Id).ToArrayAsync();
                }
                foreach (var item in userView.WarehouseIds)
                {
                    await _dbContext.UserWarehouse.AddAsync(new UserWarehouse()
                    {
                        UserId = user.Id,
                        WarehouseId = item
                    });
                }
            }
            await _dbContext.SaveChangesAsync();
            if (inventoryIds != null)
            {
                foreach (var item in inventoryIds)
                {
                    await _dbContext.UserInventory.AddAsync(new UserInventory()
                    {
                        UserId = user.Id,
                        InventoryId = item
                    });
                }
            }
            await _dbContext.SaveChangesAsync();

            user.UserName = userView.UserName;
            user.Password = userView.Password;
            user.AgentId = userView.AgentId;
            user.FleetId = userView.FleetId;
            user.Name = userView.Name;
            user.NameEn = userView.NameEn;
            user.Email = userView.Email;
            user.IsActive = userView.IsActive;
            user.IsAdmin = userView.Role.Name.Equals(Roles.administrator.ToString());
            user.ExpirationDate = userView.ExpirationDate;
            user.IsSubAdminAgent = userView.IsSubAdminAgent;
            user.UpdatedBy = userView.UpdatedBy;
            user.UpdatedDate = DateTime.Now;

            _dbContext.User.Update(user);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<GPS.Domain.Models.User> DeleteAsync(string id, string updatedBy)
        {

            var user = await _dbContext.User.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return null;
            }

            user.IsDeleted = true;
            user.UpdatedBy = updatedBy;
            user.UpdatedDate = DateTime.Now;

            var updated = _dbContext.User.Update(user);
            await _dbContext.SaveChangesAsync();

            return updated.Entity;
        }

        public async Task<int> CountAsync()
        {
            int Count = await _dbContext.User.Where(x => !x.IsDeleted).CountAsync();
            return Count;
        }

        public async Task<bool> IsUsernameExistsAsync(string Username)
        {
            var count = await _dbContext.User
                .Where(x => !x.IsDeleted && x.UserName.ToLower() == Username.ToLower())
                .CountAsync();

            return count > 0;
        }

        public async Task<bool> IsEmailExistsAsync(string Email)
        {
            var count = await _dbContext.User.Where(x => !x.IsDeleted && x.Email == Email).CountAsync();
            return count > 0;
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            var roles = await _dbContext.Role.OrderBy(x => x.Order).ToListAsync();
            return roles;
        }

        public async Task<List<UserPrivilege>> GetUserPrivilegesAsync(string userId)
        {
            var userRole = await _dbContext.UserRole.FirstOrDefaultAsync(x => x.UserId == userId);

            var userPrivilegeList = new List<UserPrivilege>();
            var privilegeTypes = await _dbContext.PrivilegeType.Where(x => x.RoleId == userRole.RoleId && (bool)x.Editable).OrderBy(x => x.Order).ToListAsync();

            var userPrivileges = await _dbContext.UserPrivilege.Where(x => x.UserId == userId).ToListAsync();
            foreach (var privilegeType in privilegeTypes)
            {
                userPrivilegeList.Add(new UserPrivilege()
                {
                    UserId = userId,
                    PrivilegeTypeId = privilegeType.Id,
                    PrivilegeType = privilegeType,
                    IsActive = (userPrivileges.FirstOrDefault(x => x.PrivilegeTypeId == privilegeType.Id)?.IsActive) ?? false
                });
            }

            return userPrivilegeList;
        }

        public async Task<bool> SaveUserPrivilegesAsync(string UserId, List<UserPrivilegeView> UserPrivileges, string UpdatedBy)
        {
            foreach (var UserPrivilegeItem in UserPrivileges)
            {
                // check if exists ? update : add
                var userPrivilege = await _dbContext.UserPrivilege
                    .FirstOrDefaultAsync(x => x.UserId == UserId && x.PrivilegeTypeId == UserPrivilegeItem.PrivilegeTypeId);

                if (userPrivilege != null)
                {
                    userPrivilege.IsActive = UserPrivilegeItem.IsActive;
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    var newUserPrivilege = new UserPrivilege()
                    {
                        UserId = UserId,
                        PrivilegeTypeId = UserPrivilegeItem.PrivilegeTypeId,
                        IsActive = UserPrivilegeItem.IsActive
                    };

                    _dbContext.UserPrivilege.Add(newUserPrivilege);
                    await _dbContext.SaveChangesAsync();
                }
            }

            return true;
        }

        public async Task<UserProfile> Login(UserLoginModel loginModel)
        {
            var user = await _dbContext.User
                .Include(x => x.Fleet)
                .FirstOrDefaultAsync(x => x.UserName.ToLower().Equals(loginModel.Username.ToLower())
                && x.Password.Equals(loginModel.Password) && !x.IsAdmin);

            if (user == null)
            {
                return null;
            }
            else if (user.IsDeleted || (bool)!user.IsActive || user.ExpirationDate.Value < DateTime.Now)
            {
                return null;
            }
            else
            {
                user.AppId = loginModel.AppId;
                user.UpdatedDate = DateTime.Now;
                user.UpdatedBy = loginModel.Username;
                _dbContext.User.Update(user);
                await _dbContext.SaveChangesAsync();

                var userWarehouses = await GetUserWarehousesAsync(user.Id);

                var userProfile = new UserProfile()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Name = user.Name,
                    NameEn = user.NameEn,
                    AgentId = user.AgentId,
                    FleetId = user.FleetId,
                    FleetName = user.Fleet?.Name,
                    UserWarehouses = userWarehouses
                };
                return userProfile;
            }
        }

        public async Task<List<int>> GetActivePrivilegeTypeIdsAsync(string userId)
        {
            var userPrivilegesTypeIds = await _dbContext.UserPrivilege
                .AsNoTracking()
                .Where(x => (bool)x.IsActive && x.UserId == userId)
                .Select(x => x.PrivilegeTypeId)
                .ToListAsync();

            return userPrivilegesTypeIds;
        }

        public async Task<GPS.Domain.Models.User> FindAsync(string Id, string Username)
        {
            var user = await _dbContext.User
                .Where(x => !x.IsDeleted &&
                (string.IsNullOrEmpty(Id) || x.Id == Id) &&
                (string.IsNullOrEmpty(Username) || x.UserName.ToLower() == Username.ToLower()))
                .Include(x => x.Agent)
                .Include(x => x.Fleet)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (user == null || user.IsDeleted)
            {
                return null;
            }

            return user;
        }

        public async Task<GPS.Domain.Models.User> GetByUserNameAndPasswordAsync(string username, string password)
        {
            var user = await _dbContext.User.Where(x => !x.IsDeleted &&
             x.UserName == username && x.Password == password)
                 .Include(x => x.Agent)
                .Include(x => x.Fleet)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<Role> GetRoleByUserIdAsync(string userId)
        {
            var userRole = await _dbContext.UserRole.FirstOrDefaultAsync(x => x.UserId == userId);
            var Role = await _dbContext.Role.FirstOrDefaultAsync(x => x.Id == userRole.RoleId);

            return Role;
        }

        public async Task<List<LookupModel>> GetUserWarehousesAsync(string userId)
        {
            var data = await _dbContext.UserWarehouse
                .Include(x => x.Warehouse)
                .Where(x => x.UserId == userId)
                 .Select(x => new LookupModel()
                 {
                     Id = x.WarehouseId,
                     Name = x.Warehouse.Name,
                 })
                .ToListAsync();

            return data;
        }
        public async Task<List<Inventory>> GetUserInventoriesAsync(string userId)
        {
            return await _dbContext.UserInventory
                .Where(x => x.UserId == userId)
                 .Include(x =>x.Inventory)
                 .Select(x =>x.Inventory)
                .ToListAsync();
        }
        public async Task<bool> ToggleMobileAlerts(UserToggleAlertsModel model)
        {
            var user = await _dbContext.User.FirstOrDefaultAsync(x => x.Id == model.UserId && x.AppId == model.AppId);
            if (user == null)
            {
                return false;
            }

            user.EnableMobileAlerts = model.Enabled;
            user.UpdatedBy = model.UserId;
            user.UpdatedDate = DateTime.Now;

            var updated = _dbContext.User.Update(user);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<DateTime?> CurrentUserExpirationDateAsync(string currentUserId)
        {
           return  await _dbContext.User.Where(x => x.Id == currentUserId).Select(x => x.ExpirationDate).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetUsersByFleetId(long fleetId)
        {
            return await _dbContext.User.Where(x => x.FleetId == fleetId).ToListAsync();
        }

        public async Task<List<Inventory>> GetUserInventoriesAndWarehouesAsync(string userId)
        {
            return await _dbContext.UserInventory.Where(x => x.UserId == userId)
                .Include(x => x.Inventory)
                .ThenInclude(x => x.Warehouse)
                .Select(x => x.Inventory)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersByIdsAsync(List<string> userIds)
        {
            return await _dbContext.User.Where(x => userIds.Contains(x.Id)).ToListAsync();
        }
    }
}
