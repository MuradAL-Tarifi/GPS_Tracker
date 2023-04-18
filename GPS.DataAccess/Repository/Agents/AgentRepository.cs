using GPS.DataAccess.Context;
using GPS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Agents
{
    public class AgentRepository : IAgentRepository
    {
        private readonly TrackerDBContext _dbContext;

        public AgentRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Agent>> GetAllAsync()
        {
            var agents = await _dbContext.Agent.Where(x => !x.IsDeleted).ToListAsync();
            return agents;
        }

        public async Task<Agent> GetFirstAgentIdAsync()
        {
            var agent = await _dbContext.Agent.Where(x => !x.IsDeleted).FirstOrDefaultAsync();
            return agent;
        }
    }
}
