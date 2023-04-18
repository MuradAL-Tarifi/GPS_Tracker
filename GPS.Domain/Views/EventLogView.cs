using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
    public class EventLogView
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string Data { get; set; }
        public string UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public UserView User { get; set; }
    }
}
