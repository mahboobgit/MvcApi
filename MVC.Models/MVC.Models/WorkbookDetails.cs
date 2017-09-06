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


        public Dictionary<string, List<string>> GetApiDetail(List<string> apis)
        {
            Dictionary<string, List<string>> allPreferenceFortheGivenApi = new Dictionary<string, List<string>>();
            foreach(string api in apis)
            {
                foreach(WorkbookTab tab in this.Tabs)
                {
                    foreach(WorkbookSubTab subtab in tab.SubTabDetails)
                    {
                        foreach(Preference pref in subtab.PreferenceDetails)
                        {
                            if (pref.IsApiExists(api))
                            {
                                
                                if (!allPreferenceFortheGivenApi.Keys.Contains(api))
                                {
                                    allPreferenceFortheGivenApi.Add(api, new List<string> { pref.PreferenceName });
                                }
                                else
                                {
                                    //Check if preference is already added
                                    if(allPreferenceFortheGivenApi.Where(k => k.Value.Contains(pref.PreferenceName)).Select(k=>k.Value).Count() == 0)
                                        allPreferenceFortheGivenApi[api].Add(pref.PreferenceName);
                                }
                            }                            
                        }
                    }
                }
            }
            return allPreferenceFortheGivenApi;
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
