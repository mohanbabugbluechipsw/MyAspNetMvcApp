using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.ViewModels
{
    //public class StoreVisitLogDto
    //{
    //    public string Rscode { get; set; }
    //    public string MrName { get; set; }
    //    public string SrName { get; set; }
    //    public string RouteName { get; set; }
    //    public string OutletCode { get; set; }
    //    public string OutletName { get; set; }
    //    public DateTime CreatedAt { get; set; }

    //    public string Rs_Email { get; set; }  // from join
    //    public string Rs_Name { get; set; }   // from joi
    //}

    public class StoreVisitLogDto
    {
        public string Rscode { get; set; }
        public string RS_Name { get; set; }
        public string MRNo { get; set; }
        public string MRName { get; set; }
        public string SrCode { get; set; }
        public string SrName { get; set; }
        public string OutletCode { get; set; }
        public string RouteName { get; set; }
        public string ChildParty { get; set; }
        public string OutletName { get; set; }
        public string ServicingPLG { get; set; }
        public string ReviewStartTime { get; set; }
        public string ReviewEndTime { get; set; }
        public string Status { get; set; }
        public string TimeSpent { get; set; }
        public DateTime TargetDate { get; set; }
        public DateTime? SendDateTime { get; set; }
        public string Rs_Email { get; set; } // for grouping & sending email
    }

}
