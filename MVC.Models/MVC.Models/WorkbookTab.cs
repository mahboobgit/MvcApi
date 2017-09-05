using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Models
{
    public class WorkbookTab
    {

        public WorkbookTab(string tabName)
        {
            SubTabDetails = new List<WorkbookSubTab>();
            TabName = tabName;
        }

        public string TabName { get; private set; }


        public List<WorkbookSubTab> SubTabDetails { get; set; }

        public bool IsSubTabExists(string subTabName)
        {
            WorkbookSubTab subTab = SubTabDetails.Find(t => t.SubTabName == subTabName);
            return subTab != null;
        }
    }
}
