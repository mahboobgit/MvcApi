using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using MVC.Helper;
using MVC.Models.Viewmodels;

namespace MVC.Controllers
{
    public class WorkbookController : Controller
    {

        [HttpGet, AllowAnonymous, Route("index")]
        public async Task<ActionResult> Index()
        {
            //string url = "http://localhost:50839/api/callinfo";
           
            WebHelper helper = new WebHelper();
            HttpResponseMessage responseMessage = await helper.CallService(IsWebApiCall: true, urlEndpoint: "callinfo", method: HttpMethod.Get, contentToPassToServer: null);

            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include };
                var workbookInfo = JsonConvert.DeserializeObject<WorkbookInfoResponse>(responseData, settings);
                Workbook workbook = new Workbook();
                if (workbookInfo.Content.Count > 0)
                {
                    workbook = workbookInfo.Content[0];
                }

                return View(workbook);
            }
            return View("Error");
                        
        }


        [HttpGet, AllowAnonymous, Route("details")]
        public async Task<ActionResult> Details(string wb)
        {
            if (wb != null)
            {
                
                WebHelper helper = new WebHelper();
                HttpResponseMessage responseMessage = await helper.CallService(IsWebApiCall: true
                                                                                , urlEndpoint: "callinfo/allpreferences"
                                                                                , method: HttpMethod.Get
                                                                                , contentToPassToServer: new Dictionary<string, string> { { "wb", wb } });

                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                    JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include };
                    var workbookInfo = JsonConvert.DeserializeObject<ApiAllDetailsResponse>(responseData, settings);

                    ApiAllDetails apiAllDetails = new ApiAllDetails();
                    if (workbookInfo.Content.Count > 0)
                    {
                        apiAllDetails = workbookInfo.Content[0];
                    }

                    return View("details", apiAllDetails);
                }
                else
                {
                    var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                    JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include };
                    var errorInfo = JsonConvert.DeserializeObject<ErrorResponse>(responseData, settings);

                    List<Error> errorList = new List<Error>();
                    if (errorInfo.Content.Count > 0)
                    {
                        errorList = errorInfo.Content;
                    }

                    return View("Error", errorList);
                }
            }
            return View("Error");
        }



        [AllowAnonymous, HttpPost, Route("search")]
        public async Task<ActionResult> Search(SearchCriteria apis)
        {

            if(apis != null)
            {
                string urlendpoint = "";
                Dictionary<string, string> content = new Dictionary<string, string>();

                content.Add("Workbook", apis.Workbook);

                if (apis.SearchOn == "apicall")
                    urlendpoint = "callinfo/apicall";                
                else                
                    urlendpoint = "callinfo/preferences";
                

                for (int i = 0; i < apis.SearchList.Count; i++)
                {
                    string key = string.Format($"SearchList[{i}]");
                    content.Add(key, apis.SearchList[i]);                   
                }
                
                


                WebHelper helper = new WebHelper();
                HttpResponseMessage responseMessage = await helper.CallService(IsWebApiCall: true
                                                                                , urlEndpoint: urlendpoint
                                                                                , method: HttpMethod.Post
                                                                                , contentToPassToServer: content);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                    if (apis.SearchOn == "apicall")
                    {
                        JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include};
                        var workbookInfo = JsonConvert.DeserializeObject<WbInfoBasedOnApiCallResponse>(responseData, settings);
                        WbInfoBasedOnApiCall info = new WbInfoBasedOnApiCall();
                        
                            
                        if (workbookInfo.Content.Count > 0)
                        {
                            info = workbookInfo.Content[0];
                        }

                        return View("apiSearch", info);
                    }
                    else
                    {
                        JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include };
                        var workbookInfo = JsonConvert.DeserializeObject<WorkbookInfoResponse>(responseData, settings);

                        Workbook workbooks = new Workbook();
                        if (workbookInfo.Content.Count > 0)
                        {
                            workbooks = workbookInfo.Content[0];
                        }

                        return View(workbooks);
                    }

                    
                }

            }

            
            return View("Error");
        }






    }
}