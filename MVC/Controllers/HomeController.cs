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
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            //string url = "http://localhost:50839/api/callinfo";
           
            WebHelper helper = new WebHelper();
            HttpResponseMessage responseMessage = await helper.CallService(IsWebApiCall: true, urlEndpoint: "callinfo", method: HttpMethod.Get, querystring: null);

            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                var workbookInfo = JsonConvert.DeserializeObject<WorkbookInfoResponse>(responseData);
                Workbook workbook = new Workbook();
                if (workbookInfo.Content.Count > 0)
                {
                    workbook = workbookInfo.Content[0];
                }

                return View(workbook);
            }
            return View("Error");
                        
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("apiInfo")]
        public async Task<ActionResult> ApiDetails(SearchForApis apis)
        {
            WebHelper helper = new WebHelper();
            HttpResponseMessage responseMessage = await helper.CallService(IsWebApiCall: true
                                                                            , urlEndpoint: "callinfo/preferences"
                                                                            , method: HttpMethod.Get
                                                                            , querystring: null);

            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                var workbookInfo = JsonConvert.DeserializeObject<WorkbookInfoResponse>(responseData);
                Workbook workbooks = new Workbook();
                if (workbookInfo.Content.Count > 0)
                {
                    workbooks = workbookInfo.Content[0];
                }

                return View(workbooks);
            }
            return View("Error");
        }

    }
}