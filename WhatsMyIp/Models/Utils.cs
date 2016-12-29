using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhatsMyIp.Models
{
    public static class Utils
    {
        public static List<List<T>> Split<T>(this List<T> source, int bulkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / bulkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static string[] Os = new string[] { "windows", "iphone;ipod;ipad", "android" };

        public static string GetBrowserName(string userAgent)
        {
            string browsers = "firefox;chrome;opera;OPR;opios;crios";
            string browser = "OTHER";
            string strUserAgent = userAgent.ToLower();
            string strFullUserAgent = userAgent;
            bool isOtherThanSafari = false;
            string[] phrases = RemoveEmptyStrings(browsers.Split(';'));

            var regex = new System.Text.RegularExpressions.Regex(@"(?:\b(MS)?IE\s+|\bTrident\/7\.0;.*\s+rv:)(\d+)");
            var match = regex.Match(strFullUserAgent);

            if (match.Success)
            {
                browser = "IE";
            }
            else
            {
                if (strUserAgent.Contains(" edge"))
                {
                    browser = "EDGE";
                }
                else
                {
                    foreach (string phrase in phrases)
                    {
                        if (strUserAgent.Contains(phrase) || strFullUserAgent.Contains(phrase))
                        {
                            if (phrase != "safari")
                                isOtherThanSafari = true;

                            browser = phrase.ToUpper();
                        }
                    }

                    if (!isOtherThanSafari && strUserAgent.Contains("safari") && !strUserAgent.Contains("mobile safari"))
                        browser = "SAFARI";
                    //Special Cases
                    if (browser == "OPR" || browser == "OPIOS")
                        browser = "OPERA";

                    if (browser == "CRIOS")
                        browser = "CHROME";

                    if (browser == "OTHER" && strUserAgent.Contains("android"))
                        browser = "ANDROID_BROWSER";
                }
            }

            return browser;
        }

        private static string[] RemoveEmptyStrings(string[] ar)
        {
            return ar.Where(val => !string.IsNullOrEmpty(val)).ToArray();
        }

        public static string FillTemplate(string template, Dictionary<string, string> placeHolders)
        {
            return placeHolders.Aggregate(template, (current, textITem) => current.Replace(textITem.Key, textITem.Value));
        }
    }
}