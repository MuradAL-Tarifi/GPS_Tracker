using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.DTO
{
    public class PagedResult<T>
    {
        public List<T> List { get; set; } = new List<T>();
        public int TotalRecords { get; set; }
    }
}
