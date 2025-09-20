using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.Models
{
    public class TblStoreVisitAnswer
    {
        public int Id { get; set; }
        public int VisitId { get; set; }
        public int QuestionId { get; set; }
        public string VisitType { get; set; }
        public string? BlobUrl { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string Rscode { get; set; }
        public string SrName { get; set; }
        public string SrCode { get; set; }
        public string RouteName { get; set; }
        public string OutletCode { get; set; }
        public string OutletName { get; set; }
        public string OutletSubType { get; set; }
        public string OutletAddress { get; set; }
        public string ChildParty { get; set; }
        public string ServicingPLG { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string DeviceInfo { get; set; }
        public string DeviceType { get; set; }


        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }


        public string ChannelType { get; set; }     // OTC, SS, etc.
        public int IsNew { get; set; }


        public Guid RowGuid { get; set; }

    }



}
