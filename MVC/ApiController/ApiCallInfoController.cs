namespace MVC.Controllers
{

    using MVC.Infrastructure;
    using MVC.Models;
    using MVC.Models.Viewmodels;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    [RoutePrefix("api/callinfo")]
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
        public async Task<HttpResponseMessage> GetAllPreferences(string wb)
        {
            
            WorkbookManager manager = new WorkbookManager();
            try
            {
                Dictionary<string, string> allWbPreferences = await manager.GetPreferenceDetailAsync(wb);
                ApiAllDetailsResponse response = new ApiAllDetailsResponse { IsError = false };
                
                if (allWbPreferences.Count > 0)
                {
                    ApiAllDetails info = new ApiAllDetails();
                    info.Workbook = wb;
                    info.WbPreferencesAndApiCalls = allWbPreferences;
                    response.Content.Add(info);
                    response.StatusCode = HttpStatusCode.OK;
                }   

                return Request.CreateResponse<ApiAllDetailsResponse>(HttpStatusCode.OK, response);
            }
            catch(Exception ex)
            {
                var errorResponse = new ErrorResponse
                {
                    IsError = true,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new List<Error>() { { new Error { Message = ex.Message, Workbook = wb } } }
                };
                
                return Request.CreateResponse<ErrorResponse>(HttpStatusCode.InternalServerError, errorResponse);
            }
        }



        [HttpPost, AllowAnonymous, Route("apicall")]
        public async Task<HttpResponseMessage> GetApiCallForPreferences(SearchCriteria searchCeiteria)
        {
            //string workbook, List<string> preferenceList

            WorkbookManager manager = new WorkbookManager();
            try
            {
                Dictionary<string, List<string>> apiList = await manager.GetPreferencesForApiCall(searchCeiteria.Workbook, searchCeiteria.SearchList);

                ApiInfoBasedOnWbResponse response = new ApiInfoBasedOnWbResponse { IsError = false };

                if (apiList.Count > 0)
                {
                    foreach (string api in apiList.Keys)
                    {
                        WbInfoBasedOnApiCall info = new WbInfoBasedOnApiCall();
                        info.WbPreferences = apiList[api];
                        info.Workbook = searchCeiteria.Workbook;
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
                    Content = new List<Error>() { { new Error { Message = ex.Message, Workbook = searchCeiteria.Workbook } } }
                };

                return Request.CreateResponse<ErrorResponse>(HttpStatusCode.InternalServerError, errorResponse);
            }
        }

       
        [HttpPost, AllowAnonymous, Route("preferences")]
        public async Task<HttpResponseMessage> GetPreferencesForApiCall (SearchCriteria searchCeiteria) //string workbook, List<string> lstApi)
        {

            WorkbookManager manager = new WorkbookManager();
            try
            {
                Dictionary<string, string> preferenceDetails = await manager.GetApiCallForPreferences(searchCeiteria.Workbook, searchCeiteria.SearchList);

                ApiBasedOnPreferenceResponse response = new ApiBasedOnPreferenceResponse { IsError = false };

                if (preferenceDetails.Count > 0)
                {
                    foreach(string preference in preferenceDetails.Keys)
                    {
                        bool newWorkBookInfo = false;

                        ApiBasedOnPreference info = null;
                        info = response.SearchBasedOnWb(searchCeiteria.Workbook);
                        if(info == null)
                        {
                            newWorkBookInfo = true;
                            info = new ApiBasedOnPreference();
                        }
                        
                        
                        var apisForPreference = new Dictionary<string, List<Api>>();
                        if (!newWorkBookInfo)
                            apisForPreference = info.PreferenceApiDetails;


                        if (!apisForPreference.ContainsKey(preference))
                        {
                            List<string> apiList = new List<string>(preferenceDetails[preference].Split('`'));
                            var apicalls = new List<Api>();
                            foreach(string apiNumber in apiList)
                            {
                                if (!string.IsNullOrEmpty(preference))
                                {   
                                    Api details = ApiDetails.Get(apiNumber);
                                    apicalls.Add(new Api { ApiNumber = apiNumber, ApiDescription = details != null ? details.ApiDescription : string.Empty });
                                }
                                    
                            }                     
                            apisForPreference.Add(preference, apicalls);
                        }

                        if (newWorkBookInfo)
                        {
                            info.PreferenceApiDetails = apisForPreference;
                            info.Workbook = searchCeiteria.Workbook;
                            response.Content.Add(info);
                        }

                        
                    }
                    response.StatusCode = HttpStatusCode.OK;
                }

                return Request.CreateResponse<ApiBasedOnPreferenceResponse>(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ErrorResponse
                {
                    IsError = true,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new List<Error>() { { new Error { Message = ex.Message, Workbook = searchCeiteria.Workbook } } }
                };

                return Request.CreateResponse<ErrorResponse>(HttpStatusCode.InternalServerError, errorResponse);
            }
        }


        
    }
}
