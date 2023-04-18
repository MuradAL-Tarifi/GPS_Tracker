using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.Models
{
    public class EventLog
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string Data { get; set; }
        public string UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public User User { get; set; }
    }
}
