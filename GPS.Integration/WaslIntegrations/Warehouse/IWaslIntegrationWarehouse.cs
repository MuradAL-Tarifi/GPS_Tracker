using GPS.Integration.WaslModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Integration.WaslIntegrations.Warehouse
{
    public interface IWaslIntegrationWarehouse
    {
        Task<WaslResponse> RegisterAsync(string companyId, WaslWarehouseModel model);

        Task<WaslResponse> UpdateAsync(string warehouseId, WaslWarehouseUpdateModel model);

        /// <summary>
        /// WAREHOUSE INQUIRY SERVICE
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<List<WaslWarehouse>> GetAsync(string companyId);

        Task<WaslResponse> DeleteAsync(string warehouseId);

        /// <summary>
        /// Inquiry
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<WaslInquiryModel> InquiryAsync(string companyId);
    }
}
