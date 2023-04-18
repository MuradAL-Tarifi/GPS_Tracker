using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Brands
{
   public interface IBrandRepository
    {
        Task<PagedResult<Brand>> SearchAsync(string SearchString, int PageNumber, int pageSize);

        Task<Brand> FindbyIdAsync(long? Id);

        Task<bool> AddAsync(Brand brand);

        Task<bool> UpdateAsync(BrandView brandView);

        Task<Brand> DeleteAsync(long Id, string UpdatedBy);

        Task<int> CountAsync();

        Task<List<Brand>> GetAllAsync();

    }
}
