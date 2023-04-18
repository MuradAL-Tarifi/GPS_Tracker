using GPS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Agents
{
    public interface IAgentRepository
    {
        Task<List<Agent>> GetAllAsync();

        Task<Agent> GetFirstAgentIdAsync();
    }
}
