using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GPS.Domain.Models
{
    public partial class Smtpsetting
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string MailAddress { get; set; }
        public int? MaxEmailNumber { get; set; }
        public int? CurrentEmailNumber { get; set; }
    }
}
