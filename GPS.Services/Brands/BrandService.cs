using AutoMapper;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using GPS.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Brands
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BrandService> _logger;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public BrandService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<BrandService> logger,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
        }

        public async Task<ReturnResult<PagedResult<BrandView>>> SearchAsync(string SearchString = "", int PageNumber = 1, int pageSize = 100)
        {
            var result = new ReturnResult<PagedResult<BrandView>>();
            try
            {
                var pagedResult = await _unitOfWork.BrandRepository.SearchAsync(SearchString, PageNumber, pageSize);

                if (pagedResult == null)
                {
                    result.DefaultNotFound();
                    return result;
                }

                var pagedListView = new PagedResult<BrandView>
                {
                    TotalRecords = pagedResult.TotalRecords,
                    List = _mapper.Map<List<BrandView>>(pagedResult.List)
                };

                result.Success(pagedListView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<BrandView>> FindbyIdAsync(long? Id)
        {
            var result = new ReturnResult<BrandView>();
            try
            {
                var brand = await _unitOfWork.BrandRepository.FindbyIdAsync(Id);

                if (brand == null || brand.IsDeleted)
                {
                    result.NotFound(_sharedLocalizer["BrandNotExists"]);
                    return result;
                }
                result.Success(_mapper.Map<Brand, BrandView>(brand));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<bool>> SaveAsync(BrandView brand)
        {
            if (brand.Id > 0)
            {
                return await UpdateAsync(brand);
            }
            else
            {
                return await AddAsync(brand);
            }
        }

        public async Task<ReturnResult<bool>> AddAsync(BrandView brandView)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var brand = _mapper.Map<BrandView, Brand>(brandView);
                brand.CreatedBy = brandView.CreatedBy;
                brand.CreatedDate = DateTime.Now;

                await _unitOfWork.BrandRepository.AddAsync(brand);

                await _unitOfWork.EventLogRepository.LogEventAsync(Event.create, brand.Id, brand, brandView.CreatedBy);

                result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
                result.Data = false;
            }
            return result;
        }

        public async Task<ReturnResult<bool>> UpdateAsync(BrandView brandView)
        {
            var result = new ReturnResult<bool>();
            try
            {
                bool updated = await _unitOfWork.BrandRepository.UpdateAsync(brandView);
                if (updated)
                {
                    await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, brandView.Id, brandView, brandView.UpdatedBy);
                }

                result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
                result.Data = false;
            }
            return result;
        }

        public async Task<ReturnResult<bool>> DeleteAsync(long Id, string UpdatedBy)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var entity = await _unitOfWork.BrandRepository.DeleteAsync(Id, UpdatedBy);

                if (entity == null)
                {
                    result.NotFound(_sharedLocalizer["BrandNotExists"]);
                    result.Data = false;
                    return result;
                }

                await _unitOfWork.EventLogRepository.LogEventAsync(Event.delete, entity.Id, entity, UpdatedBy);

                result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
                result.Data = false;
            }
            return result;
        }

        public async Task<ReturnResult<int>> CountAsync()
        {
            var result = new ReturnResult<int>();
            try
            {
                int Count = await _unitOfWork.BrandRepository.CountAsync();
                result.Success(Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<BrandView>>> GetAllAsync()
        {
            var result = new ReturnResult<List<BrandView>>();
            try
            {
                var brands = await _unitOfWork.BrandRepository.GetAllAsync();
                result.Success(_mapper.Map<List<BrandView>>(brands));
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
