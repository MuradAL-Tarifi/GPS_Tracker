using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GPS.Integration.WaslModels
{
    public class WaslInquiryModel
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
    }
}
