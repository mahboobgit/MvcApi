﻿

namespace MVC.Controllers
{
    using System;
    using MVC.Helper;
    using MVC.Models;
    using MVC.Models.Viewmodels;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    //[OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
    [RoutePrefix("workbook")]
    public class WorkbookController : AsyncController
    {


        [HttpGet, AllowAnonymous, Route("test")]
        public ActionResult Test()
        {
            return Content("Hello World");
        }


        [HttpGet, AllowAnonymous, Route("index")]
        public async Task<ActionResult> Index()
        {
            
            WebHelper helper = new WebHelper(getUrl());
            HttpResponseMessage responseMessage = new HttpResponseMessage();
           
            try
            {
                responseMessage = await helper.CallService(IsWebApiCall: true, urlEndpoint: "wbinfo", method: HttpMethod.Get, contentToPassToServer: null);
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
                else
                {
                    return ParameterError(responseMessage.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                return ParameterError(ex.Message);
            }
        }
        


        [HttpGet, AllowAnonymous, Route("details")]
        public async Task<ActionResult> Details(string wb)
        {
            if (wb != null)
            {
                
                WebHelper helper = new WebHelper(getUrl());
                HttpResponseMessage responseMessage = await helper.CallService(IsWebApiCall: true
                                                                                , urlEndpoint: "wbinfo/tabs"
                                                                                , method: HttpMethod.Get
                                                                                , contentToPassToServer: new Dictionary<string, string> { { "wb", wb } });

                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                    JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include };
                    var workbookInfo = JsonConvert.DeserializeObject<ApiTabDetailsResponse>(responseData, settings);

                    ApiTabDetails apiAllDetails = new ApiTabDetails();
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
        public async Task<ActionResult> SearchApi(SearchCriteria<string> apis)
        {

            if(apis != null)
            {
                string urlendpoint = "";
                Dictionary<string, string> content = new Dictionary<string, string>();

                content.Add("Workbook", apis.Workbook);
                urlendpoint = "wbinfo/apis";

                //if (apis.SearchOn == "apicall")
                //    urlendpoint = "callinfo/apicall";                
                //else                
                //    urlendpoint = "callinfo/preferences";


                for (int i = 0; i < apis.SearchList.Count; i++)
                {                   
                    string key = string.Format($"SearchList[{i}]");
                    content.Add(key, apis.SearchList[i]);
                }



                WebHelper helper = new WebHelper(getUrl());
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
        public async Task<ActionResult> searchpreferences(SearchCriteria<string> apis)
        {

            if (apis != null)
            {
                string urlendpoint = "";
                Dictionary<string, string> content = new Dictionary<string, string>();

                content.Add("Workbook", apis.Workbook);
                urlendpoint = "wbinfo/preferences";

                //if (apis.SearchOn == "apicall")
                //    urlendpoint = "callinfo/apicall";
                //else
                //    urlendpoint = "callinfo/preferences";


                for (int i = 0; i < apis.SearchList.Count; i++)
                {
                    string key = string.Format($"SearchList[{i}]");
                    content.Add(key, apis.SearchList[i]);
                }

                
                WebHelper helper = new WebHelper(getUrl());
                HttpResponseMessage responseMessage = await helper.CallService(IsWebApiCall: true
                                                                                , urlEndpoint: urlendpoint
                                                                                , method: HttpMethod.Post
                                                                                , contentToPassToServer: content);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include };
                    var workbookInfo = JsonConvert.DeserializeObject<ApiBasedOnPreferenceResponse>(responseData, settings);

                    List<ApiBasedOnPreference> apiCall = new List<ApiBasedOnPreference>();
                    if (workbookInfo.Content.Count > 0)
                    {
                        apiCall = workbookInfo.Content;
                    }

                    return View(apiCall);

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
                ErrorResponse errorInfo = new ErrorResponse();
                try
                {

                    JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include };
                    errorInfo = JsonConvert.DeserializeObject<ErrorResponse>(errorResponse, settings);
                    if (errorInfo.Content.Count > 0)
                    {
                        errorList = errorInfo.Content;
                    }
                }
                catch
                {
                    if (!string.IsNullOrEmpty(errorResponse))
                    {
                        errorList.Add(new Error { Message = errorResponse , Workbook = "not known"});
                    }
                }

                
            }
            return View("Error", errorList);
        }

        string getUrl()
        {
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            return baseUrl;
        }
    }
}