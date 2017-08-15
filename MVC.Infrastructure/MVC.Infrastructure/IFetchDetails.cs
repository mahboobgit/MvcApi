using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Infrastructure
{
    interface IAsyncFetchDetails
    {
        Task<Dictionary<string, string>> GetPreferenceDetailAsync();

        Task<Dictionary<string, List<string>>> GetPreferencesForApiCallAsync(List<string> apiCallValues);
        
        Task<Dictionary<string, string>> GetApiCallForPreferencesAsync(List<string> preferences);
    }
}
