using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Model_New.ViewModels
{
    public class FilterViewModel
    {
        public string SelectedMrId { get; set; }
        public string SelectedRsCode { get; set; }
        public string SelectedOutletType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public List<SelectListItem> MrNames { get; set; }
        public List<SelectListItem> RsCodes { get; set; }
        public List<SelectListItem> OutletTypes { get; set; }
        public List<OSADetailViewModel> OsaResults { get; set; }
    }


    public class OSADetailViewModel
    {
        public string RsCode { get; set; }
        public string PartyCode { get; set; }
        public string QuestionText { get; set; }
        public string Answer { get; set; }
        public DateTime AnsweredDate { get; set; }
        public string QuestionOutletType { get; set; }

    }
}
