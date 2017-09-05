

namespace MVC.Models
{

    using System.Collections.Generic;

    public class ApiTabDetails
    {
        public string Workbook { get; set; }

        public List<WorkbookTab> TabList  { get; set; }
        
    }



    public class ApiTabDetailsResponse : ApiResponseMessage<ApiTabDetails>
    {

        public ApiTabDetailsResponse()
        {
            this.InitializeContent();
        }


        public override void InitializeContent()
        {
            base.Content = new List<ApiTabDetails>();
        }
    }
}