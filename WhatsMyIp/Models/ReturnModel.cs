using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhatsMyIp.Models
{
    public class ReturnModel
    {
        public string error { get; set; }
        public string pixel { get; set; }
        public string url { get; set; }
        public string xml { get; set; }
    }
}