

namespace MVC.Models
{

    using System.Collections.Generic;

    public class ApiAllDetails
    {
        public string Workbook { get; set; }
        public Dictionary<string,string> WbPreferencesAndApiCalls  { get; set; }
        
    }



    public class ApiAllDetailsResponse : ApiResponseMessage<ApiAllDetails>
    {

        public ApiAllDetailsResponse()
        {
            this.InitializeContent();
        }


        public override void InitializeContent()
        {
            base.Content = new List<ApiAllDetails>();
        }
    }
}