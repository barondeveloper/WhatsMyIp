using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WhatsMyIp.Models;

namespace WhatsMyIp.Controllers
{
    public class TranslateController : ApiController
    {
        // GET api/values
        public async  Task<string> Get()
        {
            Visitor model = null;

            try
            {
                var CallerIp = HttpContext.Current.Request.UserHostAddress.ToString();
                var isLocal = CallerIp == "::1";
                var CallerAgent = HttpContext.Current.Request.UserAgent;
                var CalledUrl = HttpContext.Current.Request.Url.OriginalString;
                var browserName = Utils.GetBrowserName(CallerAgent);

                var referer = HttpContext.Current.Request.UrlReferrer;
                model = new Visitor { IP = isLocal ? "127.0.0.1" : CallerIp, UserAgent = CallerAgent, Browser = browserName, ReffererUrl = referer?.ToString() };
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

            try
            {
                var dc = new video_tdsEntities();
                dc.Visitors.Add(model);
                await dc.SaveChangesAsync();
            }
            catch (Exception)
            {
               
            }
            return model.IP;
        }      
    }
}
