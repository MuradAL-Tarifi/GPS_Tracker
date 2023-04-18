using GPS.Domain.DTO;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Agents
{
   public interface IAgentService
    {
        /// <summary>
        /// Get all Agents
        /// </summary>
        Task<ReturnResult<List<AgentView>>> GetAllAsync();

        /// <summary>
        /// Get the default agent Id
        /// </summary>
        Task<ReturnResult<int>> GetFirstAgentIdAsync();
    }
}
