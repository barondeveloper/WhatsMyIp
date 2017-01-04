using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WhatsMyIp.Models
{
    public class HttpServiceResponse<TResult>
    {
        public TResult Result { get; set; }
        public HttpResponseMessage Response { get; set; }
        
    }

    public static class HttpUtils
    {
      
        #region POST
        public static async Task<HttpServiceResponse<TResult>> PostServiceResponse<TRequest, TResult> ( string url, string actionUrl, TRequest value )
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.PostAsJsonAsync(actionUrl, value);
                var httpResult = new HttpServiceResponse<TResult>() { Response = response };
                if (response.IsSuccessStatusCode)
                {
                    var jsonStr = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(jsonStr))
                    {
                        //handle error
                    }
                    httpResult.Result = JsonConvert.DeserializeObject<TResult>(jsonStr);
                }
                return httpResult;
            }
        }

        public static async Task<string> PostString(string url, string json,  List<KeyValuePair<string, string>> headers = null, string failMessage = null)
        {
            return await SendAsyncGetString(url, json, HttpMethod.Post, headers, failMessage);
        }


        #endregion

        #region GET

        public static async Task<JObject> GetJson ( string url ) => await SendAsyncGetJson(url, HttpMethod.Get, null, "Fail");

        public static async Task<string> GetString ( string url ) => await SendAsyncGetString(url, HttpMethod.Get, null, "Fail");

        #endregion

        #region DELETE

        public static async Task<string> Delete(string endpoint, List<KeyValuePair<string, string>> headers = null, string OKMessage="OK" ,string failMessage = null)
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint),
                Method = HttpMethod.Delete,
            };

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return OKMessage;
            }

            return failMessage;
        }

        #endregion

        #region base


        public static async Task<string> SendAsyncGetString ( string endpoint,string json ,HttpMethod method, List<KeyValuePair<string, string>> headers = null, string failMessage = null )
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint),
                Method = method,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                return result;
            }

            return failMessage;
        }


        public static async Task<string> SendAsyncGetString ( string endpoint, HttpMethod method, List<KeyValuePair<string, string>> headers = null, string failMessage = null )
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(endpoint),
                Method = method //HttpMethod.Get,
            };

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            HttpResponseMessage response = await httpClient.SendAsync(request );

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                return result;
            }

            return failMessage;
        }

        public static async Task<JObject> SendAsyncGetJson ( string endpoint, HttpMethod method, List<KeyValuePair<string, string>> headers = null, string failMessage = null )
        {
            var json = await SendAsyncGetString(endpoint, method, headers, failMessage);

            return JObject.Parse(json);
        }


        #endregion
    }
}
