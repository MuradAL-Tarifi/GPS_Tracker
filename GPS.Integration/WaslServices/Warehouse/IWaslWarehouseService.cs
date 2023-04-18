using GPS.Domain.DTO;
using GPS.Integration.WaslModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Integration.WaslServices.Warehouse
{
    public interface IWaslWarehouseService
    {
        Task<ReturnResult<WaslResult>> RegisterAsync(string companyId, WaslWarehouseModel model);

        Task<ReturnResult<WaslResult>> UpdateAsync(string warehouseId, WaslWarehouseUpdateModel model);

        Task<ReturnResult<WaslResult>> DeleteAsync(string warehouseId);

        /// <summary>
        /// Inquiry
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<ReturnResult<WaslInquiryModel>> InquiryAsync(string companyId);
    }
}
