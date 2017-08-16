using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Models.Viewmodels
{
    public class SearchCriteria
    {
        public string SearchOn { get; set; }

        public string Workbook { get; set; }

        public List<string> SearchList { get; set; }
    }
}
