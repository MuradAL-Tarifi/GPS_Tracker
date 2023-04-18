using GPS.Domain.DTO;
using GPS.Domain.Views;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.API.Proxy
{
   public interface IUserProxy
    {
        [Post("/api/v1/account/login")]
        Task<ReturnResult<UserView>> LoginAsync(string username, string password);
    }
}
