using GPS.DataAccess.Context;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Brands
{
    public class BrandRepository : IBrandRepository
    {
        private readonly TrackerDBContext _dbContext;

        public BrandRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<Brand>> SearchAsync(string SearchString, int PageNumber, int pageSize)
        {
            var pagedList = new PagedResult<Brand>();
            var skip = (PageNumber - 1) * pageSize;

            pagedList.TotalRecords = await _dbContext.Brand.Where(x => !x.IsDeleted  &&
            (string.IsNullOrEmpty(SearchString) || (x.Name.Contains(SearchString) || x.NameEn.Contains(SearchString))))
                .CountAsync();

            pagedList.List = await _dbContext.Brand.Where(x => !x.IsDeleted  &&
                 (string.IsNullOrEmpty(SearchString) || (x.Name.Contains(SearchString) || x.NameEn.Contains(SearchString))))
                    .OrderByDescending(x => x.CreatedDate)
                    .Skip(skip).Take(pageSize)
                    .AsNoTracking().ToListAsync();

            return pagedList;
        }

        public async Task<Brand> FindbyIdAsync(long? Id)
        {
            var brand = await _dbContext.Brand.Where(x => !x.IsDeleted && x.Id == Id)
                .AsNoTracking().FirstOrDefaultAsync();

            return brand;
        }

        public async Task<bool> AddAsync(Brand brand)
        {
            await _dbContext.Brand.AddAsync(brand);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(BrandView brandView)
        {
            var brand = await _dbContext.Brand.FindAsync(brandView.Id);
            if (brand == null)
            {
                return false;
            }
            brand.Name = brandView.Name;
            brand.NameEn = brandView.NameEn;
            brand.UpdatedBy = brandView.UpdatedBy;
            brand.UpdatedDate = DateTime.Now;

            _dbContext.Brand.Update(brand);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<Brand> DeleteAsync(long Id, string UpdatedBy)
        {
            var brand = await _dbContext.Brand.FindAsync(Id);
            if (brand == null)
            {
                return null;
            }

            brand.IsDeleted = true;
            brand.UpdatedBy = UpdatedBy;
            brand.UpdatedDate = DateTime.Now;

            var deleted = _dbContext.Brand.Update(brand);
            await _dbContext.SaveChangesAsync();

            return deleted.Entity;
        }

        public async Task<int> CountAsync()
        {
            int Count = await _dbContext.Brand.Where(x => !x.IsDeleted).CountAsync();
            return Count;
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            var brand = await _dbContext.Brand.Where(x => !x.IsDeleted)
               .AsNoTracking().ToListAsync();

            return brand;
        }

    }
}
