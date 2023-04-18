
using GPS.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.Models
{
    public class WaslIntegrationLog
    {
        public long Id { get; set; }
        public WaslIntegrationLogTypeEnum WaslLogTypeLookupId { get; set; }
        public int HttpCode { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public DateTime LogDate { get; set; }
    }
}
