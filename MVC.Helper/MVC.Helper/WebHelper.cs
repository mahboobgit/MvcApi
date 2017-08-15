using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MVC.Helper
{
    public class WebHelper
    {

        string url;

        public WebHelper()
        {
            AppSettingsConfigReader appSettingReader = new AppSettingsConfigReader();
            url = appSettingReader.GetAppSettingValue("WebApiUrl");
            url = url.Length == 0 ? "http://localhost:50839" : url;
            
        }

        public async Task<HttpResponseMessage> CallService(bool IsWebApiCall, string urlEndpoint, HttpMethod method, Dictionary<string,string> querystring = null)
        {
            string urlruntime = urlEndpoint;
            
            HttpResponseMessage responseMessage;
            HttpClient client = new HttpClient();
            url = IsWebApiCall ? string.Format($"{url}/api/") : url;
            client.BaseAddress = new Uri(url);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if(method == HttpMethod.Get)
            {
                if (querystring != null && querystring.Count > 0)
                {
                    var qs = new StringBuilder();
                    foreach(var kvp in querystring)
                    {
                        qs.AppendFormat($"{kvp.Key}={kvp.Value}&");
                    }
                    string qsFinal = qs.ToString().Length > 0
                                                          ? qs.ToString().Remove(qs.ToString().Length - 1)
                                                          : "";
                                                
                    if(qsFinal.Length > 0)
                        urlruntime = string.Format($"{url}/{urlruntime}?{qsFinal}");
                }
               
                responseMessage = await client.GetAsync(urlruntime);
                return responseMessage;
            }

            

            ErrorResponse errorResponse = new ErrorResponse
            {
                IsError = true,
                StatusCode = HttpStatusCode.BadRequest,
                Content = new List<Error> { { new Error { Message = "Operation is not supported"} }}
            };
            

            responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content =   new StringContent(JsonConvert.SerializeObject(errorResponse), 
                                                Encoding.UTF8, 
                                                "application/json")  
            };
            return responseMessage;
        }
        

    }
}
