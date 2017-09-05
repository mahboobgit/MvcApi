using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Models
{
    public class WorkbookDetails
    {

        public WorkbookDetails()
        {
            Tabs = new List<WorkbookTab>();
        }

        public string WorkBookName { get; set; }


        public List<WorkbookTab> Tabs { get; set; }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public bool IsTabExists(string tabName)
        {
            WorkbookTab tab = Tabs.Find(t => t.TabName == tabName);
            return tab != null;
        }

    }



    public class WorkbookDetailsResponse : ApiResponseMessage<WorkbookDetails>
    {
        public WorkbookDetailsResponse()
        {
            this.InitializeContent();
        }

        public override void InitializeContent()
        {
            this.Content = new List<WorkbookDetails>();
        }
    }
}
