using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.Models
{
    public class EmailHistory
    {
        public long Id { get; set; }
       
        public long? AlertId { get; set; }
        
        public string Title { get; set; }
        
        public string Body { get; set; }
        
        public string ToEmails { get; set; }

        public DateTime CreatedDate { get; set; }
        
        public bool IsSent { get; set; }

        public DateTime? SentDate { get; set; }
    }
}
