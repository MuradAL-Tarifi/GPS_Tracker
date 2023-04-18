using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
   public class CompanySettingViewModel
    {
        public byte[] LogoPhotoByte { set; get; }
        public IFormFile LogoPhoto { set; get; }
        public string LogoFileBase64 { get; set; }
        [MaxLength(50)]
        public string LogoPhotoExtention { get; set; }
    }
}
