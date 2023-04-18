using GPS.Domain.DTO;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Brands
{
   public interface IBrandService
    {
        /// <summary>
        /// Get Brands, Get Brands Search Brands by Name/NameEn
        /// </summary>
        /// <param name="SearchString"></param>
        /// <param name="PageNumber"></param>
        /// <param name="PageSize"></param>
        /// <returns>List of Brands</returns>
        Task<ReturnResult<PagedResult<BrandView>>> SearchAsync(string SearchString = "", int PageNumber = 1, int pageSize = 100);

        /// <summary>
        /// Find Brands by Id
        /// </summary>
        Task<ReturnResult<BrandView>> FindbyIdAsync(long? Id);

        /// <summary>
        /// Add or Update Brands
        /// </summary>
        /// <param name="brandView"></param>
        Task<ReturnResult<bool>> SaveAsync(BrandView brandView);

        /// <summary>
        /// Delete a Brands
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="UpdatedBy"></param>
        Task<ReturnResult<bool>> DeleteAsync(long Id, string UpdatedBy);

        /// <summary>
        /// Get GrBrandsoups Count
        /// </summary>
        Task<ReturnResult<int>> CountAsync();

        /// <summary>
        /// Get Brands by BrandTypeId
        /// </summary>
        /// <param name="BrandTypeId"></param>
        Task<ReturnResult<List<BrandView>>> GetAllAsync();


    }
}
