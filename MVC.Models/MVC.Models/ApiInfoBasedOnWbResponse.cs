using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
    public class ApiInfoBasedOnWb
    {
        string Api { get; set; }
        List<string> WbPreferences { get; set; }
    }


    public class ApiInfoBasedOnWbResponse : ApiResponseMessage<WbInfoBasedOnApiCall>
    {

        public ApiInfoBasedOnWbResponse()
        {
            this.InitializeContent();
        }

        public override void InitializeContent()
        {
            this.Content = new List<WbInfoBasedOnApiCall>();
        }
    }
}