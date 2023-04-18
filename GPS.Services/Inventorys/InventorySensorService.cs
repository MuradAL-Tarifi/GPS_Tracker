using AutoMapper;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Views;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GPS.Services.Inventorys
{
    public class InventorySensorService : IInventorySensorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<InventorySensorService> _logger;

        public InventorySensorService(
             IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<InventorySensorService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ReturnResult<InventorySensorView>> FindBySensorSerialAsync(string serial)
        {
            var result = new ReturnResult<InventorySensorView>();

            try
            {
                var warehouseSensor = await _unitOfWork.InventorySensorRepository.GetBasicBySerial(serial);

                if (warehouseSensor == null || warehouseSensor.IsDeleted)
                {
                    result.NotFound("not found");
                }
                else
                {
                    result.Success(_mapper.Map<InventorySensorView>(warehouseSensor));
                }
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
