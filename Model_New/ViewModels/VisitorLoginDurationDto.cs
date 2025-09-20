using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_New.ViewModels
{
    public class VisitorLoginDurationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? BlobUrl { get; set; }
        public DateTime CaptureDate { get; set; }
        public DateTime? LogoutDate { get; set; }
        public DateTime LastActivityComputed { get; set; }
        public string Duration_HMS { get; set; } = string.Empty;
    }

}
