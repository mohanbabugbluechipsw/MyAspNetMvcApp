using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.ViewModels
{
    public class ReviewInputModel
    {
        public Guid ReviewId { get; set; } // For idempotency (client can retry safely)

        public string MrName { get; set; }
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
        public string? PhotoUrl { get; set; }

        //public string PhotoData { get; set; }

        public double? Latitude { get; set; }      // Nullable, supports saveWithoutLocation()
            public double? Longitude { get; set; }     // Nullable
            public double? Accuracy { get; set; }      // Optional, if you want to store GPS accuracy

            public string DeviceInfo { get; set; }
            public string DeviceType { get; set; }     // "Mobile", "Tablet", "Desktop"
        public string ReviewMessage { get; set; }

        public string UserId { get; set; }  // ✅ Add this

    }
}
