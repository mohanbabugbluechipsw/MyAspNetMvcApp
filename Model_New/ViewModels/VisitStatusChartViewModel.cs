using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.ViewModels
{
    public class VisitStatusChartViewModel
    {
        public string RS_Code { get; set; }
        public string RS_Name { get; set; }
        public string Region { get; set; }
        public string Area { get; set; }

        public int TotalCount { get; set; }
        public int CompletedCount { get; set; }
        public int VisitedCount { get; set; }
        public int NotVisitedCount { get; set; }
    }
}
