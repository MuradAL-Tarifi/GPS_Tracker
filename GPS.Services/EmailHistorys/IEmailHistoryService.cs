using GPS.Domain.DTO;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.EmailHistorys
{
    public interface IEmailHistoryService
    {
        //Task<ReturnResult<EmailHistoryView>> SearchAsync(long VehicleId);

        Task<ReturnResult<long>> AddAsync(EmailHistoryView emailHistoryView);

        //Task<ReturnResult<bool>> UpdateSentAsync(long id, string UpdatedBy);
    }
}
