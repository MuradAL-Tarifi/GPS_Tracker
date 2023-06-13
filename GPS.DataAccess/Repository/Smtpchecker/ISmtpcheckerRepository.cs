using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Smtpchecker
{
    public interface ISmtpcheckerRepository
    {
        Task<Domain.Models.Smtpchecker> AddAsync(Domain.Models.Smtpchecker smtpchecker);
        Task<bool> UpdateAsync(AlertSensorView alertSensorView);
        Task<Domain.Models.Smtpchecker> DeleteAsync(long itemId);

    }
}
