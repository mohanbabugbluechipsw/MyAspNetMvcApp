using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.ViewModels
{
    public class OSAReviewViewModel
    {
        public int Id { get; set; }
        public string RSCode { get; set; }
        public string PartyHLLCode { get; set; }
        public string OutletType { get; set; }
        public string QuestionText { get; set; }
        public string Answer { get; set; }
        public string FormattedDate { get; set; }
        public DateTime ReviewDate { get; set; }
        public int QuId { get; set; }

        public int TotalCount { get; set; } // Added property

    }
}
