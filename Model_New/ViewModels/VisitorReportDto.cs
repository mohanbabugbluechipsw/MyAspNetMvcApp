using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.ViewModels
{
    public class VisitorReportDto
    {
        public string Rscode { get; set; } = "";
        public string RS_Name { get; set; } = "";         // Distributor/RS Name
        public string MRNo { get; set; } = "";           // MR Employee Number
        public string MRName { get; set; } = "";
        public string SrCode { get; set; } = "";
        public string SrName { get; set; } = "";
        public string OutletCode { get; set; } = "";
        public string RouteName { get; set; } = "";
        public string ChildParty { get; set; } = "";
        public string OutletName { get; set; } = "";
        public string ServicingPLG { get; set; } = "";
        public string ReviewStartTime { get; set; } = "";
        public string ReviewEndTime { get; set; } = "";
        public string Status { get; set; } = "";          // CompletedOutlet / VisitedOutlet / N.A
        public string TimeSpent { get; set; } = "";       // e.g., "1 hr 10 min"
        public DateTime TargetDate { get; set; }          // The date of the report
    }

}
