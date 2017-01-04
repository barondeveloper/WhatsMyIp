using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhatsMyIp.Models
{
    public class UrlRequstModel
    {
        public string link { get; set; }
        public string auth { get; set; }
        public string feed { get; set; }

        public string ua { get; set; }
        public string url { get; set; }
        public string user_ip { get; set; }
        public string query { get; set; }
        public string subid { get; set; }
    }
}