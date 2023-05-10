using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.DTO
{
    public class ExportDto
    {
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
    }
}
