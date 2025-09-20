using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using System;
    using System.ComponentModel.DataAnnotations;

    public class Tbl_LogindetailsCapture
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Username { get; set; }

        public string ImageFileName { get; set; }

        public byte[]? ImageData { get; set; }  // nullable if some rows may not have image data

        public string? BlobUrl { get; set; }

        public DateTime CaptureDate { get; set; }

        public DateTime? LogoutDate { get; set; }     // new column

        public DateTime? LastActivity { get; set; }   // new column
    }


}
