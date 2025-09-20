using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.ViewModels
{
    public class ReviewContextModel
    {
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
        public Guid ReviewId { get; set; }
    }
}
