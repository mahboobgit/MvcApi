
namespace MVC.Helper
{

    using System;
    using System.Collections.Generic;
    using System.Runtime.Caching;

    public sealed class CacheManager
    {

        private static readonly CacheManager instance = new CacheManager();
      
        private static ObjectCache cache = MemoryCache.Default;
        private CacheItemPolicy policy = null;
        private CacheEntryRemovedCallback callback = null;
        
        static CacheManager() { }

        CacheManager() { }


        public static CacheManager GetInstance
        {
            get
            {
                return instance;
            }
        }

        public bool Add(string cacheKey, object cacheOject, string fileDependencyToDropCache, string workbook = null)
        {
            List<string> fileDependancy = new List<string>();
            if(!string.IsNullOrEmpty(fileDependencyToDropCache))
                fileDependancy.Add(fileDependencyToDropCache);

            policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.Default;
            if(fileDependancy.Count > 0)
                policy.ChangeMonitors.Add(new HostFileChangeMonitor(fileDependancy));
            cache.Add(cacheKey, cacheOject, policy);
            return true;
        }


        public bool Set(string cacheKey, object cacheOject, string fileDependencyToDropCache, string workbook = null)
        {
            List<string> fileDependancy = new List<string>();
            if (!string.IsNullOrEmpty(fileDependencyToDropCache))
                fileDependancy.Add(fileDependencyToDropCache);

            policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.Default;
            if (fileDependancy.Count > 0)
                policy.ChangeMonitors.Add(new HostFileChangeMonitor(fileDependancy));
            cache.Set(cacheKey, cacheOject, policy);
            return true;
        }

        public bool Exists(string cacheKey)
        {
            return cache.Contains(cacheKey);
        }
        

        public Object Get(string cacheKey)
        {            
            if(cache.Contains(cacheKey))
                return cache[cacheKey] as Object;

            return null;
        }

        public void Remove(string cacheKey)
        {
            if (cache.Contains(cacheKey))
                cache.Remove(cacheKey);            
        }

    }
}
