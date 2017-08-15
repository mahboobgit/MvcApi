using MVC.Models;

namespace MVC.Controllers
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using MVC.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using MVC.Infrastructure;
    using System.Threading.Tasks;



    public class CallInfoController : ApiController
    {

        [HttpGet, AllowAnonymous]
        public async Task<HttpResponseMessage> Workbook()
        {

            WorkbookManager wbManager = new WorkbookManager();
            List<string> workbooks = await wbManager.GetWorkbookList();

            WorkbookInfoResponse response = new WorkbookInfoResponse { IsError = false };

            if (workbooks.Count > 0)
            {
                Workbook wb = new Workbook { Workbooks = workbooks };
                response.Content.Add(wb);
                response.StatusCode = HttpStatusCode.OK;
            }

            return Request.CreateResponse<WorkbookInfoResponse>(HttpStatusCode.OK, response);
        }



        [HttpGet, AllowAnonymous, Route("allpreferences")]
        public async Task<HttpResponseMessage> GetAllPreferences(string workbook)
        {
            
            WorkbookManager manager = new WorkbookManager();
            try
            {
                Dictionary<string, string> allWbPreferences = await manager.GetPreferenceDetailAsync(workbook);
                ApiInfoBasedOnWbResponse response = new ApiInfoBasedOnWbResponse { IsError = false };
                
                if (allWbPreferences.Count > 0)
                {
                    WbInfoBasedOnApiCall info = new WbInfoBasedOnApiCall();
                    info.WbPreferences = allWbPreferences.Keys.ToList();
                    info.Workbook = workbook;
                    info.Api = "";
                    response.Content.Add(info);
                    response.StatusCode = HttpStatusCode.OK;
                }   

                return Request.CreateResponse<ApiInfoBasedOnWbResponse>(HttpStatusCode.OK, response);
            }
            catch(Exception ex)
            {
                var errorResponse = new ErrorResponse
                {
                    IsError = true,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new List<Error>() { { new Error { Message = ex.Message } } }
                };
                
                return Request.CreateResponse<ErrorResponse>(HttpStatusCode.OK, errorResponse);
            }
        }



        [HttpPost, AllowAnonymous, Route("apicall")]
        public async Task<HttpResponseMessage> GetApiCallForPreferences(string workbook, List<string> preferenceList)
        {

            WorkbookManager manager = new WorkbookManager();
            try
            {
                Dictionary<string, string> apiList = await manager.GetApiCallForPreferences(workbook, preferenceList);
                
                ApiInfoBasedOnWbResponse response = new ApiInfoBasedOnWbResponse { IsError = false };

                if (apiList.Count > 0)
                {
                    WbInfoBasedOnApiCall info = new WbInfoBasedOnApiCall();
                    info.WbPreferences = apiList.Keys.ToList();
                    info.Workbook = workbook;
                    info.Api = "";
                    response.Content.Add(info);
                    response.StatusCode = HttpStatusCode.OK;
                }

                return Request.CreateResponse<ApiInfoBasedOnWbResponse>(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ErrorResponse
                {
                    IsError = true,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new List<Error>() { { new Error { Message = ex.Message } } }
                };

                return Request.CreateResponse<ErrorResponse>(HttpStatusCode.OK, errorResponse);
            }
        }



        [HttpPost, AllowAnonymous, Route("preferences")]
        public async Task<HttpResponseMessage> GetPreferencesForApiCall (string workbook, List<string> lstApi)
        {

            WorkbookManager manager = new WorkbookManager();
            try
            {
                Dictionary<string, List<string>> apiList = await manager.GetPreferencesForApiCall(workbook, lstApi);

                ApiInfoBasedOnWbResponse response = new ApiInfoBasedOnWbResponse { IsError = false };

                if (apiList.Count > 0)
                {
                    foreach(string api in apiList.Keys)
                    {
                        WbInfoBasedOnApiCall info = new WbInfoBasedOnApiCall();
                        info.WbPreferences = apiList[api];
                        info.Workbook = workbook;
                        info.Api = api;
                        response.Content.Add(info);
                    }
                    response.StatusCode = HttpStatusCode.OK;
                }

                return Request.CreateResponse<ApiInfoBasedOnWbResponse>(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ErrorResponse
                {
                    IsError = true,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new List<Error>() { { new Error { Message = ex.Message } } }
                };

                return Request.CreateResponse<ErrorResponse>(HttpStatusCode.OK, errorResponse);
            }
        }


        
    }
}
