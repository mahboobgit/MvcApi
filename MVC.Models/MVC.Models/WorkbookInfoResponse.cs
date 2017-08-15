namespace MVC.Models
{

    using System.Collections.Generic;


    public class Workbook
    {

        public List<string> Workbooks { get; set; }

        public Workbook()
        {
            Workbooks = new List<string>();
        }
    }


    public class WorkbookInfoResponse : ApiResponseMessage<Workbook>
    {
        public WorkbookInfoResponse()
        {
            this.InitializeContent();
        }

        public override void InitializeContent()
        {
            this.Content = new List<Workbook>();
        }
    }
}