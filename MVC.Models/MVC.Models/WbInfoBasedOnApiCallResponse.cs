

namespace MVC.Models
{

    using System.Collections.Generic;

    public class WbInfoBasedOnApiCall
    {
        public string Workbook { get; set; }
        public string Api { get; set; }
        public List<string> WbPreferences { get; set; }
    }



    public class WbInfoBasedOnApiCallResponse : ApiResponseMessage<WbInfoBasedOnApiCall>
    {

        public WbInfoBasedOnApiCallResponse()
        {
            this.InitializeContent();
        }


        public override void InitializeContent()
        {
            base.Content = new List<WbInfoBasedOnApiCall>();
        }
    }
}