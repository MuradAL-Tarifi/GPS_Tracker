using AutoMapper;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Views;
using GPS.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Agents
{
    public class AgentService : IAgentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AgentService> _logger;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public AgentService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AgentService> logger,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
        }

        public async Task<ReturnResult<List<AgentView>>> GetAllAsync()
        {
            var result = new ReturnResult<List<AgentView>>();
            try
            {
                var agents = await _unitOfWork.AgentRepository.GetAllAsync();
                result.Success(_mapper.Map<List<AgentView>>(agents));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<int>> GetFirstAgentIdAsync()
        {
            var result = new ReturnResult<int>();
            try
            {
                var agent = await _unitOfWork.AgentRepository.GetFirstAgentIdAsync();
                if (agent == null)
                {
                    result.NotFound(_sharedLocalizer["NoAgents"]);
                    result.Data = -1;
                    return result;
                }

                result.Success(agent.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }
    }
}
