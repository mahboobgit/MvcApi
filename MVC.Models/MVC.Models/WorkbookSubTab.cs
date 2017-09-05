using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Models
{
    public class WorkbookSubTab
    {

        public WorkbookSubTab(string subTabName)
        {
            PreferenceDetails = new List<Preference>();
            SubTabName = subTabName;
        }

        public string SubTabName { get; private set; }

        public List<Preference> PreferenceDetails { get; set; }

    }
}
