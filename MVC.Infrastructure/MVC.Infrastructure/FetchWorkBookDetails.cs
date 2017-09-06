

namespace MVC.Infrastructure
{
    using MVC.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml.Linq;


    public class FetchWorkBookDetails : IAsyncFetchDetails
    {




        #region  ========================== Properties and Constructor ===============================


        XDocument xdoc = null;
        Dictionary<string, string> apicalls_with_preference = null;
        Dictionary<string, string> workbooksInfo = new Dictionary<string, string>();


        WorkbookDetails workbookDetails = new WorkbookDetails();
        
        public WorkbookDetails GetWorkbookDetails
        {
            get{return workbookDetails;}
        }


        public Task Initialization { get; private set; }


        public FetchWorkBookDetails(string filePath)
        {
            try
            {
                xdoc = XDocument.Load(filePath);
                //Initialization = loadWbDetails();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        #endregion


        public async Task<WorkbookDetails> FetchDetails()
        {
            workbookDetails.WorkBookName = getWBName();
            await GetTabDetails();
            return workbookDetails;
        }


        string getWBName()
        {
            string wbName = string.Empty;

            if (xdoc.Root != null && xdoc.Root.Attribute("Id") != null)
                wbName = xdoc.Root.Attribute("Id").Value;

            return wbName;
        }

        async Task GetTabDetails()
        {
            await Task.Factory.StartNew(() =>
            {
                IEnumerable<XNode> nodes = from p in xdoc.Descendants("PageConfiguration")
                                           from p1 in p.Descendants("ReportMapping")
                                           from p2 in p1.Nodes()
                                           select p2;


                foreach (XNode node in nodes)
                {
                    if (node is XElement)
                    {
                        var tabName = ((XElement)node).Attribute("HeaderTabTitle") != null ? ((XElement)node).Attribute("HeaderTabTitle").Value : "";
                        WorkbookTab tab = null;
                        if (!string.IsNullOrEmpty(tabName) && !workbookDetails.IsTabExists(tabName))
                        {
                            //Get the Tab Details.
                            tab = new WorkbookTab(tabName);
                            workbookDetails.Tabs.Add(tab);
                        }
                        else
                        {
                            tab = workbookDetails.Tabs.Find(x => x.TabName == tabName);
                        }

                        //Now get the subtab details.
                        GetSubTabDetail(node, tab);
                    }
                }
            });
        }


        void GetSubTabDetail(XNode node, WorkbookTab tab)
        {
            var subTabName = ((XElement)node).Attribute("HeaderSubTabTitle") != null ? ((XElement)node).Attribute("HeaderSubTabTitle").Value : "";
            if (!string.IsNullOrEmpty(subTabName) && !tab.IsSubTabExists(subTabName))
            {
                //Get the SubTab Details.
                WorkbookSubTab subTab = new WorkbookSubTab(subTabName);
                tab.SubTabDetails.Add(subTab);

                //Get the preference Detail
                GetSubTabPreferenceDetail(node, subTab);
            }
        }


        void GetSubTabPreferenceDetail(XNode selectedNode, WorkbookSubTab tab)
        {

            IEnumerable<XElement> elements = from p in ((XElement)selectedNode).Descendants("Widget")                                             
                                             select p;

            foreach (XElement element in elements)
            {
                string apiCalls = element.Attribute("LayoutDataIDs").Value;                
                if (string.IsNullOrEmpty(apiCalls)) continue;

                string preferenceName = element.Attribute("PrefName").Value;
                List<string> apiList = new List<string>(apiCalls.Split('`'));
                var apicalls = new List<Api>();

                foreach (string apiNumber in apiList)
                {   
                    Api details = ApiDetails.Get(apiNumber);
                    apicalls.Add(new Api { ApiNumber = apiNumber, ApiDescription = details != null ? details.ApiDescription : string.Empty });                 
                }

                Preference pref = new Preference { ApiDetails = apicalls, PreferenceName = preferenceName };

                tab.PreferenceDetails.Add(pref);
            }
        }






        #region ================================== Private Methods ==================================

        bool IsPreferenceDictionaryNeedsToBeFetched()
        {
            if (apicalls_with_preference == null)
            {
                apicalls_with_preference = new Dictionary<string, string>();
                return true;
            }

            if (apicalls_with_preference.Count == 0) return true;

            return false;

        }

        void GetAllPreferenceList()
        {
            IEnumerable<XNode> nodes = from p in xdoc.Descendants("PageConfiguration")
                                       from p1 in p.Descendants("TTSPreferenceList")
                                       from p2 in p1.Nodes()
                                       select p2;


            foreach (XNode node in nodes)
            {
                if (node is XElement)
                {
                    var attribute = ((XElement)node).Attribute("Name") != null ? ((XElement)node).Attribute("Name").Value : "";
                    if (!string.IsNullOrEmpty(attribute) && !apicalls_with_preference.ContainsKey(attribute))
                        apicalls_with_preference.Add(attribute, null);
                }
            }            
        }


        void GetAllApiCalls()
        {

            IEnumerable<XElement> elements = from p in xdoc.Descendants("PageConfiguration")
                                             from p1 in p.Descendants("ReportMapping")
                                             from p2 in p1.Descendants("Report")
                                             select p2;


            foreach (XElement element in elements)
            {

                IEnumerable<XElement> nodes = from p in element.Descendants("Widget")
                                              select p;
                foreach (XElement node in nodes)
                {
                    if (node is XElement)
                    {
                        UpdateDictionaryWithApiCalls(node.ToString());
                    }
                }
            }


        }


        void UpdateDictionaryWithApiCalls(string widget)
        {
            Regex regex = new Regex("(?<=\\s*?PrefName=\")(?<pref>.*?)(?=\")", RegexOptions.IgnoreCase);
            Match match = regex.Match(widget);
            string preferenceName = match.Success ? match.Groups["pref"].Value.Trim() : "";

            if (preferenceName.Length > 0)
            {
                regex = new Regex("(?<=\\s*?LayoutDataIDs=\")(?<api>.*?)(?=\")", RegexOptions.IgnoreCase);
                match = regex.Match(widget);
                string apiCalls = match.Success ? match.Groups["api"].Value.Trim() : "";

                if (!string.IsNullOrEmpty(apiCalls) && apicalls_with_preference.ContainsKey(preferenceName))
                {
                    apicalls_with_preference[preferenceName] = apiCalls;
                }
            }
        }


        //async Task<Dictionary<string,string>> GetWorkbookDetailsFromConfiguration()
        //{
        //    var wbWithPath = new Dictionary<string, string>();

        //    await Task.Factory.StartNew(() =>
        //    {
        //        foreach (String key in ConfigurationManager.AppSettings.AllKeys)
        //        {
        //            if (key.StartsWith("WB:") && !wbWithPath.ContainsKey(key))
        //            {
        //                wbWithPath.Add(key.Replace("WB:",""), ConfigurationManager.AppSettings[key]);
        //            }
        //        }
        //    });

        //    return wbWithPath;

        //}


        #endregion


        #region =============================== Public Methods ======================================

        public async Task<Dictionary<string, string>> GetPreferenceDetailAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                if (IsPreferenceDictionaryNeedsToBeFetched())
                {
                    GetAllPreferenceList();
                    GetAllApiCalls();
                }
            });
            
            return apicalls_with_preference;
        }
        

        public async Task<Dictionary<string, List<string>>> GetPreferencesForApiCallAsync(List<string> apiCallValues)
        {
            await GetPreferenceDetailAsync();
            var preferences = new Dictionary<string, List<string>>();

            await Task.Factory.StartNew(() =>
            {
                foreach (string api in apiCallValues)
                {
                    var match = apicalls_with_preference.Values.FirstOrDefault(k => k != null && k.Contains(api));
                    if (match != null)
                    {

                        foreach (string key in apicalls_with_preference.Keys)
                        {
                            if (apicalls_with_preference[key] != null && apicalls_with_preference[key].Contains(api))
                            {
                                if (!preferences.ContainsKey(api))
                                    preferences.Add(api, new List<string>());
                                bool addvalue = preferences[api].Count == 0 || preferences[api].Any(v => v != key);
                                if (addvalue)
                                {
                                    preferences[api].Add(key);
                                }

                            }

                        }
                    }
                }
            });
            

            return preferences;
        }


        public async Task<Dictionary<string, string>> GetApiCallForPreferencesAsync(List<string> preferences)
        {
            await GetPreferenceDetailAsync();
            var preferencesApiCallDetails = new Dictionary<string, string>();

            await Task.Factory.StartNew(() =>
            {
                foreach (string preference in preferences)
                {
                    if (apicalls_with_preference.ContainsKey(preference))
                    {
                        if (!preferencesApiCallDetails.ContainsKey(preference))
                            preferencesApiCallDetails.Add(preference, null);

                        preferencesApiCallDetails[preference] = apicalls_with_preference[preference];
                    }
                }
            });
            

            return preferencesApiCallDetails;
        }


       


        #endregion




    }
}
