using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Shared.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
