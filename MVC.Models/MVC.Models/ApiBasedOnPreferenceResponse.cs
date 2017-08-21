using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
    public class ApiBasedOnPreference
    {

        public ApiBasedOnPreference()
        {
            PreferenceApiDetails = new Dictionary<string, List<Api>>();
        }

        public string Workbook { get; set; }

        public Dictionary<string, List<Api>> PreferenceApiDetails { get; set; }
        
    }


    public class ApiBasedOnPreferenceResponse : ApiResponseMessage<ApiBasedOnPreference>
    {

        public ApiBasedOnPreference SearchBasedOnWb(string wb)
        {
            return base.Content.Where(k => k.Workbook == wb).FirstOrDefault();            
        }

        public ApiBasedOnPreferenceResponse()
        {
            this.InitializeContent();
        }

        public override void InitializeContent()
        {
            this.Content = new List<ApiBasedOnPreference>();
        }
    }
}