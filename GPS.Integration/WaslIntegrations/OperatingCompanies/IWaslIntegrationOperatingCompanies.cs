using GPS.Integration.WaslModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Integration.WaslIntegrations.OperatingCompanies
{
    public interface IWaslIntegrationOperatingCompanies
    {
        /// <summary>
        /// Register an operating company in WASL platform
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<WaslResponse> RegisterCompanyAsync(WaslRegisterCompanyModel model);

        /// <summary>
        /// Register an individual in WASL platform
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<WaslResponse> RegisterIndividualAsync(WaslIndividualModel model);

        /// <summary>
        /// Inquiry about the registration status of  an operating company in WASL 
        /// </summary>
        /// <param name="IdentityNumber"></param>
        /// <param name="CommercialRecordNumber"></param>
        /// <returns></returns>
        Task<WaslCompany> GetCompanyAsync(string IdentityNumber, string CommercialRecordNumber, string activity);


        /// <summary>
        /// Inquiry about the registration status of individual in WASL 
        /// </summary>
        /// <param name="IdentityNumber"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        Task<WaslCompany> GetIndividualAsync(string IdentityNumber, string activity);

        /// <summary>
        /// Update an operating company contact info in WASL system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<WaslResponse> UpdateContactInfoAsync(WaslUpdateCompanyModel model);

        /// <summary>
        /// Delete an operating company in WASL system
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <returns></returns>
        Task<WaslResponse> DeleteAsync(string referenceNumber, string activity);

        /// <summary>
        /// Company Inquiry
        /// </summary>
        /// <param name="IdentityNumber"></param>
        /// <param name="CommercialRecordNumber"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        Task<WaslInquiryModel> CompanyInquiryAsync(string IdentityNumber, string CommercialRecordNumber, string activity);

        /// <summary>
        /// Individual inquiry
        /// </summary>
        /// <param name="IdentityNumber"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        Task<WaslInquiryModel> IndividualInquiryAsync(string IdentityNumber, string activity);
    }
}
