

namespace MVC.Controllers
{
    using MVC.Helper;
    using MVC.Models;
    using MVC.Models.Viewmodels;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    //[OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class WorkbookController : Controller
    {

        [HttpGet, AllowAnonymous, Route("index")]
        public async Task<ActionResult> Index()
        {
            
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
                    ParameterError(responseMessage.Content.ReadAsStringAsync().Result);
                }
            }
            return ParameterError();
        }



        [AllowAnonymous, HttpPost, Route("searchapi")]
        public async Task<ActionResult> SearchApi(SearchCriteria apis)
        {

            if(apis != null)
            {
                string urlendpoint = "";
                Dictionary<string, string> content = new Dictionary<string, string>();

                content.Add("Workbook", apis.Workbook);
                urlendpoint = "callinfo/apicall";

                //if (apis.SearchOn == "apicall")
                //    urlendpoint = "callinfo/apicall";                
                //else                
                //    urlendpoint = "callinfo/preferences";
                

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
                    JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include };
                    var workbookInfo = JsonConvert.DeserializeObject<WbInfoBasedOnApiCallResponse>(responseData, settings);
                    WbInfoBasedOnApiCall info = new WbInfoBasedOnApiCall();


                    if (workbookInfo.Content.Count > 0)
                    {
                        info = workbookInfo.Content[0];
                    }
                    ModelState.Clear();
                    return View(info);                    
                }
                else
                {
                    ParameterError(responseMessage.Content.ReadAsStringAsync().Result);                    
                }
            }

            return ParameterError();
        }


        [AllowAnonymous, HttpPost, Route("searchpreferences")]
        public async Task<ActionResult> searchpreferences(SearchCriteria apis)
        {

            if (apis != null)
            {
                string urlendpoint = "";
                Dictionary<string, string> content = new Dictionary<string, string>();

                content.Add("Workbook", apis.Workbook);
                urlendpoint = "callinfo/preferences";

                //if (apis.SearchOn == "apicall")
                //    urlendpoint = "callinfo/apicall";
                //else
                //    urlendpoint = "callinfo/preferences";


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
                    JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include };
                    var workbookInfo = JsonConvert.DeserializeObject<WorkbookInfoResponse>(responseData, settings);

                    Workbook workbooks = new Workbook();
                    if (workbookInfo.Content.Count > 0)
                    {
                        workbooks = workbookInfo.Content[0];
                    }

                    return View(workbooks);

                }
                else
                {
                    ParameterError(responseMessage.Content.ReadAsStringAsync().Result);
                }

            }

            return ParameterError();
        }



        private ActionResult ParameterError(string errorResponse = null)
        {

            List<Error> errorList = new List<Error>();

            if (string.IsNullOrEmpty(errorResponse))
            {
                var error = new Error
                {
                    Workbook = "Not Set",
                    Message = "Input Parameter missing"
                };
                errorList.Add(error);
            }
            else
            {
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include };
                var errorInfo = JsonConvert.DeserializeObject<ErrorResponse>(errorResponse, settings);

                if (errorInfo.Content.Count > 0)
                {
                    errorList = errorInfo.Content;
                }
            }
            return View("Error", errorList);
        }


    }
}