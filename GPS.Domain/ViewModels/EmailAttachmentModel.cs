﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
    public class EmailAttachmentModel
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }
    }
}
