using AutoMapper;
using GPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.API.Server.Services
{
    public interface IAutoMapperConfig
    {
        IMapper CreateMapper();
    }
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<InventoryHistoryView, OnlineInventoryHistory>();
            CreateMap<OnlineInventoryHistory, InventoryHistoryView>();
            CreateMap<InventorySensor, InventorySensorView>();
        }

    }
}
