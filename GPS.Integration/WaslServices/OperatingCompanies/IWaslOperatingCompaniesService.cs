using GPS.Domain.DTO;
using GPS.Integration.WaslModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Integration.WaslServices.OperatingCompanies
{
    public interface IWaslOperatingCompaniesService
    {
        /// <summary>
        /// Inquiry about the registration status of  an operating company in WASL 
        /// </summary>
        /// <param name="identityNumber"></param>
        /// <param name="commercialRecordNumber"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        Task<ReturnResult<WaslCompany>> GetAsync(string identityNumber, string commercialRecordNumber, string activity);

        /// <summary>
        /// Register an operating company in WASL platform
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ReturnResult<WaslResult>> RegisterCompanyAsync(WaslRegisterCompanyModel model);

        /// <summary>
        /// Register an individual in WASL platform
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ReturnResult<WaslResult>> RegisterIndividualAsync(WaslIndividualModel model);

        /// <summary>
        /// Update an operating company contact info in WASL system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ReturnResult<WaslResult>> UpdateContactInfoAsync(WaslUpdateCompanyModel model);

        /// <summary>
        /// Delete an operating company in WASL system
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        Task<ReturnResult<WaslResult>> DeleteAsync(string referenceNumber, string activity);


        /// <summary>
        /// Inquiry
        /// </summary>
        /// <param name="identityNumber"></param>
        /// <param name="commercialRecordNumber"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        Task<ReturnResult<WaslInquiryModel>> InquiryAsync(string identityNumber, string commercialRecordNumber, string activity);
    }
}
