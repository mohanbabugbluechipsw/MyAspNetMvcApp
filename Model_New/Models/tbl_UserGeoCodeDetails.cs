using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.Models
{
    public class tbl_UserGeoCodeDetails
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string UserId { get; set; }   // Unique user identifier from claims or session

        [MaxLength(100)]
        public string UserName { get; set; } // Username or email

        [MaxLength(50)]
        public string Latitude { get; set; }

        [MaxLength(50)]
        public string Longitude { get; set; }

        [MaxLength(500)]
        public string DeviceInfo { get; set; }

        [MaxLength(50)]
        public string DeviceType { get; set; }  // Mobile / Tablet / Desktop

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
