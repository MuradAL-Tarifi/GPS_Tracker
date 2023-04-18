using GPS.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Accounts
{
    public class AccountRepository : IAccountRepository
    {
        private readonly TrackerDBContext _dbContext;

        public AccountRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
