using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.ViewModels
{
    public class Tbl_DetailsummaryReportDto
    {
        public string RS_Code { get; set; }        // maps to v.Rscode
        public string OutletCode { get; set; }     // maps to v.OutletCode
        public string MrName { get; set; }         // maps to v.MrName
        public string OutletName { get; set; }     // maps to v.OutletName
        public string SrCode { get; set; }         // maps to v.SrCode
        public string RouteName { get; set; }      // maps to v.RouteName
        public string ChildParty { get; set; }     // maps to v.ChildParty
        public string ServicingPLG { get; set; }   // maps to v.ServicingPLG
        public string StartDate { get; set; }      // CAST(v.StartTime AS DATE)
        public string StartTime { get; set; }      // FORMAT(v.StartTime, 'hh:mm tt')
        public string EndDate { get; set; }        // CAST(p.EndTime AS DATE)
        public string EndTime { get; set; }        // FORMAT(p.EndTime, 'hh:mm tt')
        public string Status { get; set; }         // Completed / Visited
        public string TotalTimeSpent { get; set; } // TotalTime (formatted hr/min)
    }


}
