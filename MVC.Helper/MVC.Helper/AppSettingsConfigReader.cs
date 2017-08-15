
namespace MVC.Helper
{

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;



    public class AppSettingsConfigReader
    {
        string appSettingCacheKey = "appSettings";
        Dictionary<string, string> allAppSettingsInfo = new Dictionary<string, string>();
        CacheManager cacheManager = CacheManager.GetInstance;

        public AppSettingsConfigReader()
        {

            if (!cacheManager.Exists(appSettingCacheKey))
            {
                foreach (String key in ConfigurationManager.AppSettings.AllKeys)
                {
                    if (!allAppSettingsInfo.ContainsKey(key))
                    {
                        allAppSettingsInfo.Add(key, ConfigurationManager.AppSettings[key]);
                    }
                }
                cacheManager.Add(cacheKey: appSettingCacheKey, cacheOject: allAppSettingsInfo, fileDependencyToDropCache: null, workbook: null);
            }
            else
                allAppSettingsInfo = cacheManager.Get(appSettingCacheKey) as Dictionary<string, string>;

        }

        public List<string> GetAllWorkbooks()
        {
            List<string> wb = new List<string>();
            foreach (string workbook in allAppSettingsInfo.Keys.Where(k => k.StartsWith("WB:")))
            {
                if (!wb.Exists(k => k.Contains(workbook)))
                    wb.Add(workbook.Replace("WB:", ""));
            }
            return wb;
        }

        public Dictionary<string, string> GetAllAppSettings()
        {
            return allAppSettingsInfo;
        }


        public string GetAppSettingValue(string key)
        {
            return allAppSettingsInfo.Where((k, v) => k.Key == key || k.Key == string.Format($"WB:{key}")).FirstOrDefault().Value;
        }


        public bool Contains(string key)
        {
            return allAppSettingsInfo.ContainsKey(key) || allAppSettingsInfo.ContainsKey(string.Format($"WB:{key}"));
        }

    }
}
