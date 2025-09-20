//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;


//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Model_New.Models
//{
//    public class tbl_Savereview_details
//    {




//        [Key]
//        public int Id { get; set; }

//        [MaxLength(100)]
//        public string MrName { get; set; }

//        [MaxLength(50)]
//        public string Rscode { get; set; }

//        [MaxLength(100)]
//        public string SrName { get; set; }

//        [MaxLength(50)]
//        public string SrCode { get; set; }

//        [MaxLength(100)]
//        public string RouteName { get; set; }

//        [MaxLength(50)]
//        public string OutletCode { get; set; }

//        [MaxLength(200)]
//        public string OutletName { get; set; }

//        [MaxLength(100)]
//        public string OutletSubType { get; set; }

//        [MaxLength(300)]
//        public string OutletAddress { get; set; }

//        [MaxLength(100)]
//        public string ChildParty { get; set; }

//        [MaxLength(100)]
//        public string ServicingPLG { get; set; }

//        public string PhotoData { get; set; } // Use byte[] if storing as image blob

//        public double? Latitude { get; set; }

//        public double? Longitude { get; set; }

//        [MaxLength(300)]
//        public string DeviceInfo { get; set; }

//        [MaxLength(50)]
//        public string DeviceType { get; set; }

//        [MaxLength(50)]
//        public string LocationStatus { get; set; } // Verified / Not Verified / Location Not Available

//        public double? DistanceFromOutlet { get; set; }

//        public DateTime CreatedAt { get; set; }















//}
//}



using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model_New.Models
{
    public class tbl_Savereview_details
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string MrName { get; set; }

        [MaxLength(50)]
        public string Rscode { get; set; }

        [MaxLength(100)]
        public string SrName { get; set; }

        [MaxLength(50)]
        public string SrCode { get; set; }

        [MaxLength(100)]
        public string RouteName { get; set; }

        [MaxLength(50)]
        public string OutletCode { get; set; }

        [MaxLength(200)]
        public string OutletName { get; set; }

        [MaxLength(100)]
        public string OutletSubType { get; set; }

        [MaxLength(300)]
        public string OutletAddress { get; set; }

        [MaxLength(100)]
        public string ChildParty { get; set; }

        [MaxLength(100)]
        public string ServicingPLG { get; set; }

        // Change from string to byte[]
        public byte[] PhotoData { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [MaxLength(300)]
        public string DeviceInfo { get; set; }

        [MaxLength(50)]
        public string DeviceType { get; set; }

        [MaxLength(50)]
        public string LocationStatus { get; set; }

        public double? DistanceFromOutlet { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid ReviewId { get; set; }


        [MaxLength(100)]
        public string UserId { get; set; }

        [MaxLength(500)]
        public string ReviewMessage { get; set; }

        [MaxLength(500)]
        public string PhotoUrl { get; set; }

        public bool IsCompleted { get; set; } // ✅ For resume logic
    }
}

