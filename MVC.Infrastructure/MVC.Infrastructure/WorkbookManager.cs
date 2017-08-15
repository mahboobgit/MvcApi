
namespace MVC.Infrastructure
{
    using MVC.Helper;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public class WorkbookManager
    {

        string wbMgrCacheKey = "workbookDetails";
        AppSettingsConfigReader appSettingReader;
        CacheManager cacheMgr = CacheManager.GetInstance;

        Dictionary<string, WorkBookDetails> workbookDetails = new Dictionary<string, WorkBookDetails>();


        public WorkbookManager()
        {
            appSettingReader = new AppSettingsConfigReader();

            if (!cacheMgr.Exists(wbMgrCacheKey))
                cacheMgr.Add(wbMgrCacheKey, workbookDetails, null, null);
            else
                workbookDetails = cacheMgr.Get(wbMgrCacheKey) as Dictionary<string, WorkBookDetails>;
            
        }

        
        public async Task<List<string>> GetWorkbookList()
        {
            List<string> workbooks;
            workbooks = await Task.Factory.StartNew(() =>
            {
                return appSettingReader.GetAllWorkbooks();
            });

            return workbooks;
        }

        WorkBookDetails GetWorkbookDetail(string workbook)
        {
            WorkBookDetails wbDetail;
            
            if (workbookDetails.ContainsKey(workbook))
                wbDetail = workbookDetails[workbook];
            else
            {
                if (appSettingReader.Contains(workbook))
                {
                    wbDetail = new WorkBookDetails(appSettingReader.GetAppSettingValue(workbook));
                    workbookDetails.Add(workbook, wbDetail);
                    cacheMgr.Set(wbMgrCacheKey, workbookDetails, null, null);
                }
                else
                    throw new Exception(string.Format($"{workbook} key is not defined in Web.Config file"));
            }

            return wbDetail;
        }

        public async Task<Dictionary<string, string>> GetPreferenceDetailAsync(string workbook)
        {
            var detail = new Dictionary<string, string>();
            WorkBookDetails wbDetail = GetWorkbookDetail(workbook);
            detail = await wbDetail.GetPreferenceDetailAsync();
            return detail;
        }

        public async Task<Dictionary<string, string>> GetApiCallForPreferences(string workbook, List<string> preferences)
        {
            WorkBookDetails wbDetail = GetWorkbookDetail(workbook);
            Dictionary<string, string> apicalls_with_preference = await wbDetail.GetApiCallForPreferencesAsync(preferences);
            return apicalls_with_preference;
        }

        public async Task<Dictionary<string,List<string>>> GetPreferencesForApiCall(string workbook, List<string> apiCalls)
        {
            WorkBookDetails wbDetail = GetWorkbookDetail(workbook);
            Dictionary<string, List<string>> preferences_for_apicalls = await wbDetail.GetPreferencesForApiCallAsync(apiCalls);
            return preferences_for_apicalls;
        }



    }
}