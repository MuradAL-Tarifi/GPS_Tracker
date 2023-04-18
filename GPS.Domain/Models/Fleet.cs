using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GPS.Domain.Models
{
    public class Fleet
    {
        public long Id { get; set; }
        public int AgentId { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string ManagerEmail { get; set; }
        public string ManagerMobile { get; set; }
        public string SupervisorEmail { get; set; }
        public string SupervisorMobile { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string TaxRegistrationNumber { get; set; }
        public string CommercialRegistrationNumber { get; set; }
        public byte[] LogoPhotoByte { set; get; }
        public string LogoPhotoExtention { get; set; }
        public Agent Agent { get; set; }
        //public List<FleetDetails> FleetDetails { get; set; }
    }
}
