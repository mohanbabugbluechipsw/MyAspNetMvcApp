using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.Models
{
    public class TblOutletViewAnswer
    {
        public int Id { get; set; }
        public string OutletCode { get; set; }
        public string VisitType { get; set; }
        public string Format { get; set; }
        public string OptionType { get; set; } // MD / Sachet / Category
        public string OptionName { get; set; }
        public bool IsSelected { get; set; }

        public string Rscode { get; set; }
        public string SrName { get; set; }
        public string SrCode { get; set; }
        public string RouteName { get; set; }
        public string OutletName { get; set; }
        public string OutletSubType { get; set; }
        public string OutletAddress { get; set; }
        public string ChildParty { get; set; }
        public string ServicingPLG { get; set; }


        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }

        public DateTime CreatedDate { get; set; }




    }

}
