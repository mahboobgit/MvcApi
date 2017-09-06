using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Models.Viewmodels
{
    public class SearchCriteria <T> where T : class
    {
        public SearchCriteria()
        {
            SearchList = new List<T>();
        }
        public string SearchOn { get; set; }

        public string Workbook { get; set; }

        public List<T> SearchList { get; set; }
    }
}
