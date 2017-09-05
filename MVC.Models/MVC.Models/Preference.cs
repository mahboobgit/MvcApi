using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Models
{
    public class Preference
    {

        public Preference()
        {
            ApiDetails = new List<Api>();
        }

        public string PreferenceName { get; set; }

        public List<Api> ApiDetails { get; set; }


        public bool IsApiExists(string apiNumber)
        {
            Api foundApi = ApiDetails.Find(x => x.ApiNumber == apiNumber);
            return foundApi != null;
        }
    }
}
