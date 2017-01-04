using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using WhatsMyIp.Models;

namespace WhatsMyIp.Controllers
{
    [RoutePrefix("Translate")]
    public class TranslateController : ApiController
    {
        // GET api/values
        [HttpGet, Route("", Name = "getIP")]
        public async Task<string> Get()
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

        [HttpPost, Route("", Name = "getIPAndUrl")]
        public async Task<ReturnModel> GetUrl(UrlRequstModel settings)
        {
            Visitor visitor = null;
            var returnModel = new ReturnModel();
            bool isLocal = false;
            try
            {
                var CallerIp = HttpContext.Current.Request.UserHostAddress.ToString();
                isLocal = CallerIp == "::1";
                var CallerAgent = HttpContext.Current.Request.UserAgent;
                var CalledUrl = HttpContext.Current.Request.Url.OriginalString;
                var browserName = Utils.GetBrowserName(CallerAgent);

                var referer = HttpContext.Current.Request.UrlReferrer;
                visitor = new Visitor { IP = isLocal ? "127.0.0.1" : CallerIp, UserAgent = CallerAgent, Browser = browserName, ReffererUrl = referer?.ToString() };
            }
            catch (Exception ex)
            {
                returnModel.error = ex.Message;
                return returnModel;
            }


            string path = HttpContext.Current.Server.MapPath("~/Content/config.json");
            var config = path.ReadConfig<UrlRequstModel>();
            var link = config.link;
            link = link + "?feed=" + config.feed;
            link = link + "&auth=" + config.auth;
            link = link + "&subid=" + settings.subid;
            link = link + "&ua=" + settings.ua;
            link = link + "&url=" + settings.url;
            link = link + "&user_ip=" + (isLocal ? visitor.IP : "185.120.124.62");
            link = link + "&query=" + settings.query;

            var getDataTask = Task.Run(async () => { return await HttpUtils.GetString(link); });
            var saveToDBTask = Task.Run(async () =>
            {
                if(isLocal)
                {
                    var dc = new video_tdsEntities();
                    dc.Visitors.Add(visitor);
                    await dc.SaveChangesAsync();
                }               
            });

            await Task.WhenAll(getDataTask, saveToDBTask);

            var result = getDataTask.Result;

            XDocument doc = XDocument.Parse(result);
            var listing = doc.Element("result").Element("listing");
            returnModel.url = listing.Attribute("url").Value;
            returnModel.pixel = listing.Attribute("pixel").Value;

            return returnModel;

        }
    }
}
