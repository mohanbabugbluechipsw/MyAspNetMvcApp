using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.ViewModels
{
    public class QuestionLinkDto
    {
        public int PostVisitQuestionId { get; set; }
        public string OutletCode { get; set; }
        public string UserId { get; set; }

        public int IsNew { get; set; } // 1 = checked, 0 = unchecked
        public Guid RowGuid { get; set; }


    }
}
