using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.ViewModels
{
    public class StoreVisitFormModel
    {
        public int VisitId { get; set; }
        public string VisitType { get; set; } // Pre-Visit or Post-Visit''

        public string ChannelType { get; set; }
        //ChannelType = channelType,

        public List<StoreVisitAnswerModel> Answers { get; set; }
    }
}
